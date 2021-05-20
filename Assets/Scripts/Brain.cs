using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataSet
{
    public double[] input;
    public double[] output;
}

[RequireComponent(typeof(Snake))]
public class Brain : MonoBehaviour
{
    [Header("Manual Control")]
    public bool m_UseManualControl;

    private Snake m_Snake;

    [Header("Sensor")]
    public GameObject m_SensorPrefab;
    public float m_SensorAngle = 360.0f;
    public float m_SensorAngleOffset = 0.0f;
    public int m_SensorNumber = 8;
    public float m_SensorHeightOffset = 0.2f;
    public List<Sensor> m_Sensors = new List<Sensor>();

    [Header("Neural Network")]
    public bool m_UseTrain = true;
    public bool m_LoadNet;
    public string m_Filename = "net.data";
    public double m_LearnRate = 0.01;
    public int m_HiddenLayerAmount = 200;
    public int m_OutputLayerAmount = 4;
    public double m_Threshold = 0.5;
    private MultilayerPerceptronNetwork m_Net;

    [Header("Training")]
    public int m_BatchSize = 8;
    private int m_BatchCount = 0;
    public List<DataSet> m_Samples = new List<DataSet>();

    private void Start()
    {
        m_Snake = GetComponent<Snake>();
        InitializeSensors();
        InitializeNet();
    }

    public void InitializeNet()
    {
        if (m_LoadNet)
        {
            LoadNet();
        }
        else
        {
            m_Net = new MultilayerPerceptronNetwork(
                m_Sensors.Count,
                m_HiddenLayerAmount,
                m_OutputLayerAmount,
                m_LearnRate
            );
        }
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
            foodScript.m_CollisionLayer = 1 << 6;

            var wallSensor = Instantiate(m_SensorPrefab, position + height, Quaternion.Euler(rotation), transform);
            var wallScript = wallSensor.GetComponent<Sensor>();
            wallScript.m_CollisionLayer = 1 << 7;

            var snakeSensor = Instantiate(m_SensorPrefab, position - height, Quaternion.Euler(rotation), transform);
            var snakeScript = snakeSensor.GetComponent<Sensor>();
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

        if (!m_UseTrain) return;        
        var outputs = new double[4] { m_Snake.MoveUp, m_Snake.MoveRight, m_Snake.MoveDown, m_Snake.MoveLeft };
        var data = new DataSet() { input = inputs, output = outputs };
        Train(data);
    }

    private void NeuralControl()
    {
        var outputs = m_Net.GetOuputs();
        m_Snake.MoveUp = outputs[0] >= m_Threshold ? 1.0f : 0.0f;
        m_Snake.MoveRight = outputs[1] >= m_Threshold ? 1.0f : 0.0f;
        m_Snake.MoveDown = outputs[2] >= m_Threshold ? 1.0f : 0.0f;
        m_Snake.MoveLeft = outputs[3] >= m_Threshold ? 1.0f : 0.0f;
    }

    private void ManualControl()
    {
        if (Input.GetKeyDown(KeyCode.W)) m_Snake.MoveUp = 1.0f;
        if (Input.GetKeyDown(KeyCode.D)) m_Snake.MoveRight = 1.0f;
        if (Input.GetKeyDown(KeyCode.S)) m_Snake.MoveDown = 1.0f;
        if (Input.GetKeyDown(KeyCode.A)) m_Snake.MoveLeft = 1.0f;
    }

    private void Update()
    {
        m_UseTrain = m_UseManualControl;
        if (m_UseManualControl)
            ManualControl();
        else
            NeuralControl();

        if (Input.GetKeyDown(KeyCode.S)) SaveNet();
        if (Input.GetKeyDown(KeyCode.L)) LoadNet();
    }

    public string LocalPath
    {
        get
        {
#if UNITY_EDITOR
            return $"{Application.dataPath}/{m_Filename}";
#else
            return $"{Application.dataPath}/../{m_Filename}";
#endif
        }
    }

    public void SaveNet()
    {
        Debug.Log($"[SAVE] {LocalPath}");
        using StreamWriter writer = new StreamWriter(LocalPath);
        var json = JsonUtility.ToJson(m_Net);
        writer.Write(json);
    }

    public void LoadNet()
    {
        Debug.Log($"[LOAD] {LocalPath}");
        using StreamReader reader = new StreamReader(LocalPath);
        var json = reader.ReadToEnd();
        m_Net = JsonUtility.FromJson<MultilayerPerceptronNetwork>(json);
    }

    public void Train(DataSet data)
    {
        if (!m_Samples.Contains(data))
        {
            m_Samples.Add(data);
            m_BatchCount++;
        }

        if (m_BatchCount != m_BatchSize) return;
        m_BatchCount = 0;
        foreach (var sample in m_Samples)
            m_Net.Backward(sample.input, sample.output);
    }

    private void OnEnable()
    {
        Snake.OnMove += OnMove;
        Snake.OnDie += OnDie;
    }

    private void OnDisable()
    {
        Snake.OnMove -= OnMove;
        Snake.OnDie -= OnDie;
    }

    private void OnDie()
    {
        Debug.Log("Die");
    }
}
