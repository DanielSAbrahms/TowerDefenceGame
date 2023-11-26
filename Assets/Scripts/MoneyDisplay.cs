using System;
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    private TextMeshProUGUI moneyText;

    private void Awake()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
    }

    public void UpdateMoneyDisplay(int newAmount)
    {
        moneyText.text = "Money: $" + newAmount;
    }
}