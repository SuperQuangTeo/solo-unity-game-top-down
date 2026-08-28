using System;
using UnityEngine;

public class TestCurrencySource : MonoBehaviour
{
    public event Action<int> OnCurrencyChanged;

    [SerializeField] private int currentCurrency;
    public int CurrentCurrency => currentCurrency;

    [ContextMenu("Add Test Coin")]
    private void AddTestCoin()
    {
        currentCurrency++;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
}