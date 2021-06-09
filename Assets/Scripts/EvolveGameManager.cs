using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EvolveGameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject m_Food;
    public GameObject m_SnakePrefab;

    [Header("Gameplay")]
    public GameObject m_Origin;

    [Header("UI")]
    public LevelUI m_LevelUI;
    public ScoreUI m_ScoreUI;
    public Text m_DebugUI;

    [Header("Genetic Algorithm")]
    public string m_Filename = "exp";
    public float m_TimeTrial = 20.0f;
    [Range(5, 1000)]
    public int m_PopulationSize = 50;
    public int m_ChromosomeLength = 1004;
    [Range(2, 5)]
    public int m_TournamentSize = 3;
    [Range(0.0f, 1.0f)]
    public float m_ElitismRate = 0.1f;
    [Range(0.0f, 1.0f)]
    public float m_MutateRate = 0.01f;
    [Range(10, 10000)]
    public int m_MaxGeneration = 100;
    [Range(0.0f, 1.0f)]
    public double m_AlfaBlendCrossover = 0.2f;

    private int m_Generation = 1;
    private int m_CurrentChromosome = 0;
    private float m_AvgFitness = 0.0f;
    private float m_MaxFitness = 0.0f;
    private List<Chromosome> m_Population = new List<Chromosome>();
    private float m_ElapsedTime = 0.0f;

    private void Start()
    {
        Directory.CreateDirectory($"{Application.dataPath}/../Save");
        InitRandomPopulation();
        CreateSnakeByChromosome();
        ResetGame();
    }

    public void InitRandomPopulation()
    {
        for (int i = 0; i < m_PopulationSize; i++)
        {
            var chromosome = new Chromosome(m_ChromosomeLength);
            m_Population.Add(chromosome);
        }
    }

    public void CreateSnakeByChromosome()
    {
        var snake = Instantiate(m_SnakePrefab);
        snake.transform.position = m_Origin.transform.position;

        var brain = snake.GetComponentInChildren<EvolveBrain>();
        var chromosome = m_Population[m_CurrentChromosome];
        brain.m_Chromosome = chromosome;

        var snakeScript = snake.GetComponentInChildren<Snake>();
        snakeScript.m_TimeToKill = m_TimeTrial;
    }

    public void ResetGame()
    {
        m_ScoreUI.m_Score = 0;
        m_LevelUI.m_Level = 0;
        OnSpawnFood();
    }

    public void Update()
    {
        if (!m_DebugUI) return;

        m_DebugUI.text = "";
        m_DebugUI.text += $"Generation: {m_Generation} / {m_MaxGeneration}\n";
        m_DebugUI.text += $"Chromosome: {m_CurrentChromosome} / {m_PopulationSize}\n";
        m_DebugUI.text += $"Average Fitness [{m_Generation}]: {m_AvgFitness:0.0}\n";
        m_DebugUI.text += $"Best Fitness [{m_Generation}]: {m_MaxFitness:0.0}\n";
        m_DebugUI.text += $"Elapsed Time: {Time.unscaledTime:0}";
    }

    private void OnDie()
    {
        m_Population[m_CurrentChromosome].Fitness = m_ScoreUI.m_Score;

        m_CurrentChromosome++;
        if (m_CurrentChromosome < m_PopulationSize)
        {
            CreateSnakeByChromosome();
            ResetGame();
        }
        else
        {
            NextGeneration();
            if (m_Generation > m_MaxGeneration)
            {
                QuitGame();
            }
            else
            {
                m_CurrentChromosome = 0;
                CreateSnakeByChromosome();
                ResetGame();
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    private void OnEnable()
    {
        Food.OnFoodEaten += OnSpawnFood;
        Snake.OnDie += OnDie;
    }

    private void OnDisable()
    {
        Food.OnFoodEaten -= OnSpawnFood;
        Snake.OnDie -= OnDie;
    }

    public void OnSpawnFood()
    {
        var x = Random.Range(-18, 18) + 0.5f;
        var z = Random.Range(-10, 10) + 0.5f;
        var position = new Vector3(x, 0.5f, z);
        Instantiate(m_Food, position, Quaternion.identity);
    }

    public Chromosome Selection()
    {
        var candidates = new List<Chromosome>();
        for (int i = 0; i < m_TournamentSize; i++)
        {
            int index = Helper.RandomInt(m_PopulationSize);
            candidates.Add(m_Population[index]);
        }

        candidates.Sort();
        return candidates[0];
    }

    public float EvaluateLevel(Chromosome chromosome)
    {
        return chromosome.Fitness;
    }

    public List<Chromosome> Elitism()
    {
        var count = (int)(m_PopulationSize * m_ElitismRate);
        m_Population.Sort();

        var chromosomes = new List<Chromosome>();
        for (int i = 0; i < count; i++)
            chromosomes.Add(m_Population[i].Clone() as Chromosome);

        return chromosomes;
    }

    public void Save(bool append)
    {
        if (string.IsNullOrEmpty(m_Filename)) return;
        Debug.Log($"AVG: {m_AvgFitness}\tMAX: {m_MaxFitness}");
        using StreamWriter file = new StreamWriter($"{ Application.dataPath }/../Save/{m_Filename}.xls", append);
        m_AvgFitness = m_Population.Average(x => x.Fitness);
        m_MaxFitness = m_Population.Max(x => x.Fitness);
        file.WriteLine($"{m_AvgFitness.ToString(System.Globalization.CultureInfo.CurrentCulture)}\t{m_MaxFitness.ToString(System.Globalization.CultureInfo.CurrentCulture)}");
    }

    public void NextGeneration()
    {
        Save(m_Generation > 1);

        var newPopulation = Elitism();
        while (newPopulation.Count < m_PopulationSize)
        {
            Chromosome parent1 = Selection();
            Chromosome parent2 = Selection();

            Chromosome child1 = parent1.Crossover(parent2, m_AlfaBlendCrossover);
            child1.Mutate(m_MutateRate);

            if (!newPopulation.Contains(child1)) newPopulation.Add(child1);

            if (newPopulation.Count < m_PopulationSize)
            {
                Chromosome child2 = parent2.Crossover(parent1, m_AlfaBlendCrossover);
                child2.Mutate(m_MutateRate);

                if (!newPopulation.Contains(child2)) newPopulation.Add(child2);
            }
        }

        m_Population = newPopulation;
        m_Generation++;
    }
}
