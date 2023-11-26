using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    private TextMeshProUGUI gameoverText;

    private void Start()
    {
        gameoverText = gameObject.GetComponent<TextMeshProUGUI>();
        gameoverText.text = "Game Over";
        gameObject.SetActive(false);
    }
}