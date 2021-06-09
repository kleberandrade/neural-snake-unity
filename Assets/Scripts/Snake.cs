using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Snake : MonoBehaviour
{
    public delegate void OnMoveHandler();
    public static event OnMoveHandler OnMove;
    public static event OnMoveHandler OnDie;

    public float MoveLeft { get; set; }
    public float MoveRight { get; set; }
    public float MoveUp { get; set; }
    public float MoveDown { get; set; }

    public float m_MaxTimeRate = 0.5f;
    public float m_MinTimeRate = 0.05f;
    public float m_MaxFoodAmount = 70.0f;
    public Body m_FirstBody;

    private int m_FoodAmount = 0;
    private float m_ElapsedTime = 0.0f;
    private float m_TimeRate = 0.0f;
    private Vector3 m_Movement = Vector3.right;

    public float m_TimeToKill = 20.0f;
    private float m_ElapsedTimeToKill = 0.0f;
    private bool m_Alive = true;

    private void Start()
    {
        var body = GetComponent<Rigidbody>();
        body.isKinematic = true;
    }

    private void Update()
    {
        if (m_ElapsedTimeToKill >= m_TimeToKill)
            Kill();

        m_ElapsedTimeToKill += Time.deltaTime;
    }

    private void LateUpdate()
    {
        m_TimeRate = Mathf.Lerp(m_MaxTimeRate, m_MinTimeRate, m_FoodAmount / m_MaxFoodAmount);
        m_ElapsedTime += Time.deltaTime;
        if (m_ElapsedTime < m_TimeRate) return;

        if (MoveUp > 0.0f) m_Movement = new Vector3(0, 0, 1);
        else if (MoveRight > 0.0f) m_Movement = new Vector3(1, 0, 0);
        else if (MoveDown > 0.0f) m_Movement = new Vector3(0, 0, -1);
        else if (MoveLeft > 0.0f) m_Movement = new Vector3(-1, 0, 0);

        OnMove?.Invoke();

        m_FirstBody.ChangePosition(transform.position);
        transform.position += m_Movement;

        Reset();
    }

    private void Reset() => m_ElapsedTime = MoveRight = MoveLeft = MoveDown = MoveUp = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food")) return;
        Kill();
    }

    private void Kill()
    {
        if (!m_Alive) return;

        m_Alive = false;
        OnDie?.Invoke();
        m_FirstBody.Obliterate();
        Destroy(gameObject);
    }
}


