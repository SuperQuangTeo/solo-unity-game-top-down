using System;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Resource State")]
    [SerializeField] private int currentCoins;
    [SerializeField] private int currentKeys;
    [SerializeField] private int currentBombs;

    public event Action<int> OnCoinsChanged;

    public event Action<int> OnKeysChanged;

    public event Action<int> OnBombsChanged;

    public int CurrentCoins => currentCoins;

    public int CurrentKeys => currentKeys;

    public int CurrentBombs => currentBombs;

    private void Awake()
    {
        currentCoins = 0;
        currentKeys = 0;
        currentBombs = 0;
    }

    [ContextMenu("Add Test Coin")]
    public void AddCoin()
    {
        currentCoins ++;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
    }


    public void AddKeys(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        currentKeys += amount;
        OnKeysChanged?.Invoke(currentKeys);
    }


    public void AddBombs(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        currentBombs += amount;
        OnBombsChanged?.Invoke(currentBombs);
    }
}