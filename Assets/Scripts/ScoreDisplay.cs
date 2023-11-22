using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnScoreChanged += UpdateScoreDisplay;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
    }

    public void UpdateScoreDisplay(int newScore)
    {
        scoreText.text = "Score: " + newScore.ToString();
    }
}
