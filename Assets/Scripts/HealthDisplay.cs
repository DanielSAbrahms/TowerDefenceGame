using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnHealthChanged += UpdateHealthDisplay;
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnHealthChanged -= UpdateHealthDisplay;
    }

    public void UpdateHealthDisplay(int newScore)
    {
        scoreText.text = "Health: " + newScore.ToString();
    }
}
