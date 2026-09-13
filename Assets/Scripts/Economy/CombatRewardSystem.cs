using UnityEngine;

public class CombatRewardSystem : MonoBehaviour
{
    [SerializeField] private CheeseWallet wallet;
    [SerializeField] private PlayerProgress playerProgress;

    public void RewardSuccessfulHit(AttackSource source)
    {
        int reward = source == AttackSource.Manual
            ? playerProgress.ManualCoinsPerHit
            : playerProgress.AutomaticCoinsPerHit;

        wallet.AddCoins(reward);
    }

}
