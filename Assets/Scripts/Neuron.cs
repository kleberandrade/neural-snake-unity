[System.Serializable]
public class Neuron
{
    public double m_Bias = 1;
    public double[] m_Inputs;
    public double[] m_Weights;
    public double m_Output;

    public double m_LearnRate;
    public double m_Error;
    public double m_DesiredOutput;
    public double m_BackPropagatedError;

    public Neuron() { }

    public Neuron(double learnRate)
    {
        m_LearnRate = learnRate;
    }

    public Neuron(double[] inputs, double learnRate)
    {
        m_Inputs = inputs;
        m_LearnRate = learnRate;
    }

    public void InitWeights(double[] weights)
    {
        m_Weights = weights;
    }

    public void RandomWeight()
    {
        m_Weights = new double[m_Inputs.Length + 1];
        for (int i = 0; i < m_Weights.Length; i++)
        {
            m_Weights[i] = Helper.Random();
        }
    }

    public void Forward()
    {
        double sum = m_Weights[0] * m_Bias;
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            sum += m_Weights[i + 1] * m_Inputs[i];
            //sum += m_Weights[i] * m_Inputs[i];
        }
        m_Output = System.Math.Tanh(sum);
    }

    public void CalculateError()
    {
        m_Error = m_DesiredOutput - m_Output;
    }

    public void CalculateBackPropagatedError()
    {
        m_BackPropagatedError = (1.0 - m_Output * m_Output) * m_Error;
    }

    public void WeightAdjustement()
    {
        m_Weights[0] += m_LearnRate * m_Bias * m_BackPropagatedError;
        for (int i = 0; i < m_Inputs.Length; i++)
            m_Weights[i + 1] += m_LearnRate * m_Inputs[i] * m_BackPropagatedError;
    }

    public void Backward(double[] inputs, double desiredOutput)
    {
        m_Weights = inputs;
        m_DesiredOutput = desiredOutput;
        Forward();
        CalculateError();
        CalculateBackPropagatedError();
        WeightAdjustement();
    }

}
