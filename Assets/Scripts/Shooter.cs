using UnityEngine;

[RequireComponent(typeof(Perceptron))]
public class Shooter : MonoBehaviour
{
    public GameObject[] m_Ball;
    public float m_Force = 1000.0f;
    private Perceptron m_Brain;

    private void Start()
    {
        m_Brain = GetComponent<Perceptron>();
    }

    private void Throw(int x1, int x2, int y)
    {
        var position = Camera.main.transform.position;
        var rotation = Camera.main.transform.rotation;
        var ball = Instantiate(m_Ball[y], position, rotation);
        var forward = Camera.main.transform.forward;
        ball.GetComponent<Rigidbody>().AddForce(forward * m_Force);
        m_Brain.SendInput(x1, x2, y);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            Throw(0, 0, 0);
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Throw(0, 1, 1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Throw(1, 0, 1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Throw(1, 1, 1);
    }
}
