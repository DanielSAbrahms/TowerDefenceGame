using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(true);
    }

    private void Start()
    {
        GameManager.Instance.OnHealthChanged += UpdateHealthDisplay;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnHealthChanged -= UpdateHealthDisplay;
    }

    public void UpdateHealthDisplay(int newScore)
    {
        scoreText.text = "Health: " + newScore;
    }
}