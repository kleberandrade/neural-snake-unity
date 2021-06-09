using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Snake))]
public class EvolveBrain : MonoBehaviour
{
    private Snake m_Snake;

    [Header("Sensor")]
    public GameObject m_SensorPrefab;
    public float m_SensorAngle = 360.0f;
    public float m_SensorAngleOffset = 0.0f;
    public int m_SensorNumber = 8;
    public float m_SensorHeightOffset = 0.2f;
    public float m_FoodSensorDistance = 20.0f;
    public float m_WallSensorDistance = 5.0f;
    public float m_BodySensorDistance = 4.0f;
    public List<Sensor> m_Sensors = new List<Sensor>();

    [Header("Neural Network")]
    public int m_HiddenLayerAmount = 10;
    public int m_OutputLayerAmount = 4;
    public double m_Threshold = 0.5;
    private MultilayerPerceptronNetwork m_Net;

    [Header("Genetic Algorithm")]
    public Chromosome m_Chromosome;

    private void Start()
    {
        m_Snake = GetComponent<Snake>();
        InitializeSensors();
        InitializeChromosome();
        InitializeNet();
    }

    public void InitializeChromosome()
    {
        var inputLength = (m_Sensors.Count + 1) * m_HiddenLayerAmount;
        var amount = (inputLength + 1) * m_OutputLayerAmount;
        m_Chromosome = new Chromosome(amount);
    }

    public void InitializeNet()
    {
        m_Net = new MultilayerPerceptronNetwork(
            m_Sensors.Count,
            m_HiddenLayerAmount,
            m_OutputLayerAmount,
            m_Chromosome.m_Genes.ToArray()
        );
    }

    private void InitializeSensors()
    {
        for (int index = 0; index < m_SensorNumber; index++)
        {
            var position = transform.position;
            var angleY = (m_SensorAngle / m_SensorNumber) * index + m_SensorAngleOffset;
            var rotation = new Vector3(0, angleY, 0);

            var height = new Vector3(0, m_SensorHeightOffset, 0);

            var foodSensor = Instantiate(m_SensorPrefab, position, Quaternion.Euler(rotation), transform);
            var foodScript = foodSensor.GetComponent<Sensor>();
            foodScript.m_MaxDistance = m_FoodSensorDistance;
            foodScript.m_CollisionLayer = 1 << 6;

            var wallSensor = Instantiate(m_SensorPrefab, position + height, Quaternion.Euler(rotation), transform);
            var wallScript = wallSensor.GetComponent<Sensor>();
            wallScript.m_MaxDistance = m_WallSensorDistance;
            wallScript.m_CollisionLayer = 1 << 7;

            var snakeSensor = Instantiate(m_SensorPrefab, position - height, Quaternion.Euler(rotation), transform);
            var snakeScript = snakeSensor.GetComponent<Sensor>();
            snakeScript.m_MaxDistance = m_BodySensorDistance;
            snakeScript.m_CollisionLayer = 1 << 8;

            m_Sensors.Add(foodScript);
            m_Sensors.Add(wallScript);
            m_Sensors.Add(snakeScript);
        }
    }

    private void OnMove()
    {
        var inputs = new double[m_Sensors.Count];
        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = m_Sensors[i].GetDistance();

        m_Net.Calculate(inputs);
    }

    private void NeuralControl()
    {
        var outputs = m_Net.GetOuputs();
        m_Snake.MoveUp = outputs[0] >= m_Threshold ? 1.0f : 0.0f;
        m_Snake.MoveRight = outputs[1] >= m_Threshold ? 1.0f : 0.0f;
        m_Snake.MoveDown = outputs[2] >= m_Threshold ? 1.0f : 0.0f;
        m_Snake.MoveLeft = outputs[3] >= m_Threshold ? 1.0f : 0.0f;
    }

    private void Update()
    {
        NeuralControl();
    }

    private void OnEnable()
    {
        Snake.OnMove += OnMove;
    }

    private void OnDisable()
    {
        Snake.OnMove -= OnMove;
    }

}
