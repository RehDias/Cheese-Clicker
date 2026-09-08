using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets _i { get; private set; }

    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private RectTransform damageCanvas;

    public DamagePopup DamagePopupPrefab => damagePopupPrefab;
    public RectTransform DamageCanvas => damageCanvas;

    private void Awake()
    {
        _i = this;
    }
}
