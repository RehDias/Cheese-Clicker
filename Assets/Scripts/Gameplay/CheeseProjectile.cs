using UnityEngine;

public class CheeseProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float speed = 8f;
    [SerializeField, Min(0.1f)] private float maxLifetime = 5f;

    private CheeseController target;
    private int damage;
    private AttackSource attackSource;

    public void Launch(CheeseController cheese, int hitDamage, AttackSource source)
    {
        target = cheese;
        damage = hitDamage;
        attackSource = source;

        Destroy(gameObject, Mathf.Max(0.1f, maxLifetime));
    }

    private void Update()
    {
        if (target == null || !target.IsAlive)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 destination = target.HitPosition;
        transform.position = Vector3.MoveTowards(
            transform.position, destination, Mathf.Max(0.1f, speed) * Time.deltaTime);

        if ((transform.position - destination).sqrMagnitude <= 0.0001f)
        {
            int appliedDamage = target.TakeDamage(damage, destination);

            if (appliedDamage > 0)
                RewardSuccessfulHit();

            enabled = false;
            Destroy(gameObject);
        }
    }

    private void RewardSuccessfulHit()
    {
        GameAssets assets = GameAssets.Instance;

        if (assets != null && assets.CombatRewards != null)
        {
            assets.CombatRewards.RewardSuccessfulHit(attackSource);
        }
    }
}
