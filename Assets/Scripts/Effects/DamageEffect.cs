using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float lifetime = 0.5f;

    public static void Create(Vector3 position)
    {
        GameAssets assets = GameAssets.Instance;
        if (assets == null || assets.DamageEffectPrefab == null)
            return;

        Instantiate(assets.DamageEffectPrefab, position, Quaternion.identity, assets.EffectsParent);
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
