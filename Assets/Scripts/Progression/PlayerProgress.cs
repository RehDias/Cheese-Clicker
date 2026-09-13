using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    [Header("Currency Rewards")]
    [SerializeField, Min(1)] private int manualCoinsPerHit = 1;
    [SerializeField, Min(0)] private int automaticCoinsPerHit = 1;

    [Header("Combat")]
    [SerializeField, Range(0, MaxAttackSpeedLevel)] private int attackSpeedLevel;
    [SerializeField, Min(0f)] private float speedIncreasePerLevel = 0.2f;
    [SerializeField, Range(0, MaxAttackDamageLevel)] private int attackDamageLevel;
    [SerializeField, Min(0f)] private float damageIncreasePerLevel = 0.25f;

    [Header("Unlocks")]
    [SerializeField] private bool automaticAttackUnlocked;
    [SerializeField] private bool shooterUnlocked;
    [SerializeField] private bool mageUnlocked;

    public const int MaxAttackSpeedLevel = 3;
    public const int MaxAttackDamageLevel = 6;
    public event Action ProgressChanged;

    public int AttackSpeedLevel => attackSpeedLevel;
    public int AttackDamageLevel => attackDamageLevel;
    public bool CanUpgradeAttackSpeed => attackSpeedLevel < MaxAttackSpeedLevel;
    public bool CanUpgradeAttackDamage => attackDamageLevel < MaxAttackDamageLevel;
    public int ManualCoinsPerHit => manualCoinsPerHit;
    public int AutomaticCoinsPerHit => automaticCoinsPerHit;
    public float AttackSpeedMultiplier => 1f + Mathf.Clamp(attackSpeedLevel, 0, MaxAttackSpeedLevel) * speedIncreasePerLevel;
    public float AttackDamageMultiplier => 1f + Mathf.Clamp(attackDamageLevel, 0, MaxAttackDamageLevel) * damageIncreasePerLevel;
    public bool AutomaticAttackUnlocked => automaticAttackUnlocked;
    public bool ShooterUnlocked => shooterUnlocked;
    public bool MageUnlocked => mageUnlocked;

    public bool TryAddAttackSpeedLevel()
    {
        if (!CanUpgradeAttackSpeed)
            return false;

        attackSpeedLevel++;
        ProgressChanged?.Invoke();
        return true;
    }

    public bool TryAddAttackDamageLevel()
    {
        if (!CanUpgradeAttackDamage)
            return false;

        attackDamageLevel++;
        ProgressChanged?.Invoke();
        return true;
    }

    public bool TryUnlockAutomaticAttack()
    {
        if (automaticAttackUnlocked)
            return false;

        automaticAttackUnlocked = true;
        ProgressChanged?.Invoke();
        return true;
    }

    public bool TryUnlockShooter()
    {
        if (shooterUnlocked)
            return false;

        shooterUnlocked = true;
        ProgressChanged?.Invoke();
        return true;
    }

    public bool TryUnlockMage()
    {
        if (mageUnlocked)
            return false;

        mageUnlocked = true;
        ProgressChanged?.Invoke();
        return true;
    }

}
