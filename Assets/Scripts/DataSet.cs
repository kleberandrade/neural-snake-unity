using System;

[System.Serializable]
public class DataSet : IEquatable<DataSet>
{
    public double[] input;
    public double[] output;

    private bool IsEqual(double a, double b)
    {
        var epsilon = 0.0001;
        return a >= b - epsilon && a <= b + epsilon;
    }

    public bool Equals(DataSet other)
    {
        for (int i = 0; i < output.Length; i++)
            if (!IsEqual(output[i], other.output[i])) return false;

        for (int i = 0; i < input.Length; i++)
            if (!IsEqual(input[i], other.input[i])) return false;

        return true;
    }
}