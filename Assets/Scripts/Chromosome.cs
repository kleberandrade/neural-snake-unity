using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class Chromosome : ICloneable, IComparable<Chromosome>
{
    public List<double> m_Genes = new List<double>();

    public double this[int index]
    {
        get { return m_Genes[index]; }
        set { m_Genes[index] = value; }
    }

    public int Length => m_Genes.Count;

    public float Fitness { get; set; } = 0.0f;

    public Chromosome(int length)
    {
        for (int i = 0; i < length; i++)
            m_Genes.Add(Helper.Random());
    }

    public Chromosome(Chromosome chromosome)
    {
        foreach (int gene in chromosome.m_Genes)
            m_Genes.Add(gene);
    }

    public object Clone()
    {
        return new Chromosome(this);
    }

    public Chromosome Crossover(Chromosome otherParent, double alfa)
    {
        Chromosome child = Clone() as Chromosome;
        for (int i = 0; i < child.Length; i++)
            child[i] = child[i] - alfa * (otherParent[i] - child[i]);
        return child;
    }

    public Chromosome Mutate(float mutationRate)
    {
        for (int i = 0; i < Length; i++)
        {
            if (Helper.RandomFloat() < mutationRate)
                m_Genes[i] = Helper.Random();
        }

        return this;
    }

    public int CompareTo(Chromosome other)
    {
        if (Fitness > other.Fitness)
            return -1;
        else if (Fitness < other.Fitness)
            return 1;
        else
            return 0;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        m_Genes.ForEach(gene => builder.Append((char)(65 + gene)));
        return builder.ToString();
    }
}