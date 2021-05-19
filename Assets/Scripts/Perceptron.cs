using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TrainingSet
{
    public double[] input;
    public double output;
}

public class Perceptron : MonoBehaviour
{
    [Header("Neural Setup")]
    public double m_Bias = -1.0;
    public double[] m_Weights = { 2.0, 2.0 };
    public double m_LearnRate = 0.1;

    [Header("Target")]
    public GameObject m_Npc;
    public float m_JumpForce = 2000.0f;

    [Header("Backup")]
    public string m_Filename = "net.dat";

    private double m_TotalError = 0.0;
    private List<TrainingSet> m_Samples = new List<TrainingSet>();

    public string Path => $"{Application.dataPath}/{m_Filename}";

    private double StepFunction(double output)
    {
        return output <= 0.0 ? 0.0 : 1.0;
    }

    public void Save()
    {
        var file = File.CreateText(Path);
        file.WriteLine($"{m_Weights[0]};{m_Weights[1]};{m_Bias}") ;
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Path))
        {
            var file = File.OpenText(Path);
            var line = file.ReadLine();
            string[] weights = line.Split(';');
            m_Weights[0] = double.Parse(weights[0]);
            m_Weights[1] = double.Parse(weights[1]);
            m_Bias = double.Parse(weights[2]);
            file.Close();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) Save();
        if (Input.GetKeyDown(KeyCode.L)) Load();
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_Samples.Clear();
            Initialize();
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_Weights[0] = Random.Range(-1.0f, 1.0f);
        m_Weights[1] = Random.Range(-1.0f, 1.0f);
        m_Bias = Random.Range(-1.0f, 1.0f);
    }

    public void Train()
    {
        for (int index = 0; index < m_Samples.Count; index++)
        {
            double output = Calculate(index);
            double error = m_Samples[index].output - output;
            m_TotalError += Mathf.Abs((float)error);
            for (int j = 0; j < m_Weights.Length; j++)
            {
                double input = m_Samples[index].input[j];
                m_Weights[j] = m_Weights[j] + m_LearnRate * error * input;
            }
            m_Bias += error;
        }
    }

    public double Calculate(double[] inputs, double[] weights)
    {
        double u = 0.0;
        for (int i = 0; i < inputs.Length; i++)
            u += inputs[i] * weights[i];
        u += m_Bias;
        return StepFunction(u);
    }

    public double Calculate(int index)
    {
        return Calculate(m_Samples[index].input, m_Weights);
    }

    public void SendInput(double input1, double input2, double output)
    {
        double y = Calculate(new double[2]{ input1, input2 }, m_Weights);
        if (y == 1) m_Npc.GetComponent<Rigidbody>().AddForce(Vector3.up * m_JumpForce);
        m_Samples.Add(
            new TrainingSet()
            {
                input = new double[2] { input1, input2 },
                output = output
            }
        );
        Train();
    }
}
