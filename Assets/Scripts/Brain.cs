using UnityEngine;

[RequireComponent(typeof(Snake))]
public class Brain : MonoBehaviour
{
    [Header("Manual Control")]
    public bool m_UseManualControl;

    private Snake m_Snake;

    private void Start()
    {
        m_Snake = GetComponent<Snake>();
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

    private void OnMove()
    {
        Debug.Log("Move");
    }

    private void OnDie()
    {
        Debug.Log("Die");
    }

    private void ManualControl()
    {
        if (Input.GetKeyDown(KeyCode.W)) m_Snake.MoveUp =  1.0f;
        if (Input.GetKeyDown(KeyCode.D)) m_Snake.MoveRight = 1.0f;
        if (Input.GetKeyDown(KeyCode.S)) m_Snake.MoveDown = 1.0f;
        if (Input.GetKeyDown(KeyCode.A)) m_Snake.MoveLeft = 1.0f;
    }

    private void Update()
    {
        if (m_UseManualControl) ManualControl();
    }
}
