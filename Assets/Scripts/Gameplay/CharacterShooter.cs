using UnityEngine;

public class CharacterShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private CheeseProjectile projectilePrefab;
    [SerializeField, Min(1)] private int damage = 100;
    [SerializeField, Min(0.1f)] private float attackInterval = 1f;
    [Tooltip("Disable this when an attack animation event calls Fire instead.")]
    [SerializeField] private bool automaticFire = true;

    private float nextAttackTime;

    private void Update()
    {
        if (!automaticFire || Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + Mathf.Max(0.1f, attackInterval);
        Fire();
    }

    public void Fire()
    {
        CheeseController target = null;
        foreach (CheeseController cheese in FindObjectsByType<CheeseController>())
        {
            if (cheese.IsAlive)
            {
                target = cheese;
                break;
            }
        }

        FireAt(target);
    }

    public void FireAt(CheeseController target)
    {
        if (firePoint == null || projectilePrefab == null || target == null || !target.IsAlive)
            return;

        GameAssets assets = GameAssets.Instance;
        Transform parent = assets != null ? assets.EffectsParent : null;
        CheeseProjectile projectile = Instantiate(
            projectilePrefab, firePoint.position, Quaternion.identity, parent);
        projectile.Launch(target, damage);
    }
}
