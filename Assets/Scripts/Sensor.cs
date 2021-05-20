using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float m_MaxDistance = 10.0f;
    public Color m_Color = Color.red;
    public LayerMask m_CollisionLayer;

    private void FixedUpdate()
    {
        GetDistance();
    }

    public float GetDistance()
    {
        var origin = transform.position;
        var direction = transform.forward;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, m_MaxDistance, m_CollisionLayer))
        {
            Draw(origin, direction * hit.distance);
            return Mathf.Clamp01(Vector3.Distance(origin, hit.point) / m_MaxDistance);
        }

        Draw(origin, direction * m_MaxDistance);
        return 1.0f;
    }

    public void Draw(Vector3 position, Vector3 direction)
    {
        Debug.DrawRay(position, direction, m_Color);
    }
}
