using System;
using UnityEngine;

public class CheeseWallet : MonoBehaviour
{
    [SerializeField] private long startingCoins;

    private long balance;
    public event Action<long> BalanceChanged;
    public long Balance => balance;

    private void Awake()
    {
        balance = Math.Max(0, startingCoins);
    }

    public void AddCoins(long amount)
    {
        if (amount <= 0)
            return;

        balance += amount;
        BalanceChanged?.Invoke(balance);
    }

    public bool CanAfford(long price)
    {
        return price >= 0 && balance >= price;
    }

    public bool TrySpend(long price)
    {
        if (!CanAfford(price))
            return false;

        balance -= price;
        BalanceChanged?.Invoke(balance);
        return true;
    }
}
