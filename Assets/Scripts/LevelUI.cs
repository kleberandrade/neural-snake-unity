using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public int m_Level = 0;
    private Text m_Text;

    private void Start()
    {
        m_Text = GetComponent<Text>();
        m_Text.text = m_Level.ToString("00");
    }

    private void OnEnable()
    {
        Food.OnFoodEaten += OnFoodEaten;
    }

    private void OnDisable()
    {
        Food.OnFoodEaten -= OnFoodEaten;
    }

    private void OnFoodEaten()
    {
        m_Level++;
        m_Text.text = m_Level.ToString("00");
    }
}
