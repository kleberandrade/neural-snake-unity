using UnityEngine;

public class Food : MonoBehaviour
{
    public delegate void OnFoodEatenHandler();
    public static event OnFoodEatenHandler OnFoodEaten;

    private void Start()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Snake")) return;
        OnFoodEaten?.Invoke();
        Destroy(gameObject);
    }
}
