[System.Serializable]
public class MultilayerPerceptronNetwork
{
    public double m_LearnRate;
    public double[] m_Inputs;
    public Neuron[] m_HiddenLayer;
    public Neuron[] m_OutputLayer;
    public double[] m_DesiredOutputs;

    public MultilayerPerceptronNetwork() { }

    public MultilayerPerceptronNetwork(int inputNumber, int hiddenNumber, int outputNumber, double learnRate)
    {
        m_LearnRate = learnRate;

        m_HiddenLayer = new Neuron[hiddenNumber];
        for (int i = 0; i < hiddenNumber; i++)
        {
            m_HiddenLayer[i] = new Neuron(m_LearnRate);
            m_HiddenLayer[i].m_Inputs = new double[inputNumber];
            m_HiddenLayer[i].RandomWeight();
        }

        m_OutputLayer = new Neuron[outputNumber];
        for (int i = 0; i < outputNumber; i++)
        {
            m_OutputLayer[i] = new Neuron(m_LearnRate);
            m_OutputLayer[i].m_Inputs = new double[hiddenNumber];
            m_OutputLayer[i].RandomWeight();
        }
    }

    public void Forward()
    {
        double[] hiddenOutput = new double[m_HiddenLayer.Length];
        for (int i = 0; i < m_HiddenLayer.Length; i++)
        {
            m_HiddenLayer[i].Forward();
            hiddenOutput[i] = m_HiddenLayer[i].m_Output;
        }

        SetInputOnOuputLayer(hiddenOutput);
        for (int i = 0; i < m_OutputLayer.Length; i++)
        {
            m_OutputLayer[i].Forward();
        }
    }

    public void SetInputOnHiddenLayer(double[] inputs)
    {
        for (int i = 0; i < m_HiddenLayer.Length; i++)
            m_HiddenLayer[i].m_Inputs = inputs;
    }

    public void SetInputOnOuputLayer(double[] inputs)
    {
        for (int i = 0; i < m_OutputLayer.Length; i++)
            m_OutputLayer[i].m_Inputs = inputs;
    }

    public double[] GetOuputs()
    {
        double[] outputs = new double[m_OutputLayer.Length];
        for (int i = 0; i < m_OutputLayer.Length; i++)
            outputs[i] = m_OutputLayer[i].m_Output;
        return outputs;
    }

    public double[] Calculate(double[] inputs)
    {
        SetInputOnHiddenLayer(inputs);
        Forward();
        return GetOuputs();
    }

    public void CalculateHiddenLayerErrors()
    {
        for (int i = 0; i < m_HiddenLayer.Length; i++)
        {
            var sum = 0.0;
            for (int j = 0; j < m_OutputLayer.Length; j++)
                sum += m_OutputLayer[j].m_BackPropagatedError * m_OutputLayer[j].m_Weights[i + 1];

            m_HiddenLayer[i].m_Error = sum;
            m_HiddenLayer[i].CalculateBackPropagatedError();
        }
    }

    public void Backward(double[] inputs, double[] desiredOutputs)
    {
        m_Inputs = inputs;
        m_DesiredOutputs = desiredOutputs;

        SetInputOnHiddenLayer(inputs);
        Forward();

        for (int i = 0; i < m_OutputLayer.Length; i++)
        {
            m_OutputLayer[i].m_DesiredOutput = desiredOutputs[i];
            m_OutputLayer[i].CalculateError();
            m_OutputLayer[i].CalculateBackPropagatedError();
        }

        CalculateHiddenLayerErrors();

        foreach (var neuron in m_HiddenLayer)
            neuron.WeightAdjustement();

        foreach (var neuron in m_OutputLayer)
            neuron.WeightAdjustement();
    }
}
