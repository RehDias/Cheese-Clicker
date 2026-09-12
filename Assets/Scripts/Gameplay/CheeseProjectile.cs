using UnityEngine;

public class CheeseProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float speed = 8f;
    [SerializeField, Min(0.1f)] private float maxLifetime = 5f;

    private CheeseController target;
    private int damage;

    public void Launch(CheeseController cheese, int hitDamage)
    {
        target = cheese;
        damage = hitDamage;
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
            target.TakeDamage(damage, destination);
            enabled = false;
            Destroy(gameObject);
        }
    }
}
