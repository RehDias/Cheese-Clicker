using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    [Header("Currency Rewards")]
    [SerializeField, Min(1)] private int manualCoinsPerHit = 1;
    [SerializeField, Min(0)] private int automaticCoinsPerHit = 1;

    [Header("Combat")]
    [SerializeField, Min(0)] private int attackSpeedLevel;
    [SerializeField, Min(0f)] private float speedIncreasePerLevel = 0.2f;

    public int ManualCoinsPerHit => manualCoinsPerHit;
    public int AutomaticCoinsPerHit => automaticCoinsPerHit;
    public float AttackSpeedMultiplier => 1f + attackSpeedLevel * speedIncreasePerLevel;

    public void AddAttackSpeedLevel()
    {
        attackSpeedLevel++;
    }
}
