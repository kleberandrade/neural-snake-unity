using UnityEngine;

public class NeuralGameManager : MonoBehaviour
{
    public GameObject m_Food;

    private void Start()
    {
        OnSpawnFood();
    }

    private void OnEnable()
    {
        Food.OnFoodEaten += OnSpawnFood;
    }

    private void OnDisable()
    {
        Food.OnFoodEaten -= OnSpawnFood;
    }

    public void OnSpawnFood()
    {
        var x = Random.Range(-18, 18) + 0.5f;
        var z = Random.Range(-10, 10) + 0.5f;
        var position = new Vector3(x, 0.5f, z);
        Instantiate(m_Food, position, Quaternion.identity);
    }
}
