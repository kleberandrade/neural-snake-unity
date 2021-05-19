public class Helper
{
    private static System.Random m_Random;

    private static void Initiliaze()
    {
        if (m_Random == null)
        {
            var seed = System.DateTime.Now.Millisecond;
            m_Random = new System.Random(seed);
        }
    }

    public static double Random()
    {
        Initiliaze();
        return m_Random.Next(-10000, 10000) / 10000.0;
    }
}
