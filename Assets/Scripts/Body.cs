using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Body : MonoBehaviour
{
    public GameObject m_BodyPrefab;
    public Body m_NextBody;
    private bool m_FoodEaten;

    private void Start()
    {
        var body = GetComponent<Rigidbody>();
        body.isKinematic = true;
    }

    private void OnFoodEaten()
    {
        m_FoodEaten = true;
    }

    public void ChangePosition(Vector3 nextPosition)
    {
        if (m_NextBody)
            m_NextBody.ChangePosition(transform.position);
        else if (m_FoodEaten)
            CreateNextBody();
        
        transform.position = nextPosition;
    }

    private void CreateNextBody()
    {
        var body = Instantiate(m_BodyPrefab, transform.parent);
        body.transform.position = transform.position;
        m_NextBody = body.GetComponent<Body>();
    }

    public void Obliterate()
    {
        StartCoroutine(Obliterating());
    }

    public IEnumerator Obliterating()
    {
        yield return new WaitForEndOfFrame();
        m_NextBody?.Obliterate();
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        Food.OnFoodEaten += OnFoodEaten;
    }

    private void OnDisable()
    {
        Food.OnFoodEaten -= OnFoodEaten;
    }
}
