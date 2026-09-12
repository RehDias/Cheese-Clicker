using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    [Header("Cheese Assets")]
    [SerializeField] private GameObject cheesePrefab;
    [SerializeField] private Sprite[] cheeseSprites;

    [Header("Damage Assets")]
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private DamageEffect damageEffectPrefab;

    [Header("Scene References")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private CheeseSpawner cheeseSpawner;
    [SerializeField] private RectTransform damageCanvas;
    [SerializeField] private Transform effectsParent;
    [SerializeField] private HealthBar cheeseHealthBar;

    public GameObject CheesePrefab => cheesePrefab;
    public Sprite[] CheeseSprites => cheeseSprites;
    public DamagePopup DamagePopupPrefab => damagePopupPrefab;
    public DamageEffect DamageEffectPrefab => damageEffectPrefab;
    public Camera WorldCamera => worldCamera;
    public CheeseSpawner CheeseSpawner => cheeseSpawner;
    public RectTransform DamageCanvas => damageCanvas;
    public Transform EffectsParent => effectsParent;
    public HealthBar CheeseHealthBar => cheeseHealthBar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Only one GameAssets should be active in the scene.", this);
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
