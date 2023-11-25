using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    private TextMeshProUGUI moneyText;

    private void Start()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
    }

    public void UpdateMoneyDisplay(int newAmount)
    {
        moneyText.text = "Money: $" + newAmount.ToString();
    }
}
