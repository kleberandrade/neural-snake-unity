public class Helper
{
    private static System.Random m_Random;

    private static void Initialize()
    {
        if (m_Random == null)
        {
            var seed = System.DateTime.Now.Millisecond;
            m_Random = new System.Random(seed);
        }
    }

    public static double Random()
    {
        Initialize();
        return m_Random.Next(-10000, 10000) / 10000.0;
    }

    public static int RandomInt(int max)
    {
        Initialize();
        return m_Random.Next(max);
    }

    public static int RandomInt(int min, int max)
    {
        Initialize();
        return m_Random.Next(min, max);
    }

    public static float RandomFloat()
    {
        Initialize();
        return (float)m_Random.NextDouble();
    }
}
