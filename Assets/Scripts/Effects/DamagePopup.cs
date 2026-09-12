using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    [SerializeField] private float lifetime = 1.3f;
    [SerializeField] private float floatSpeed = 20f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 30f);

    private float displayDuration;
    private Color colorText;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
    }

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        GameAssets assets = GameAssets.Instance;
        if (assets == null || assets.DamagePopupPrefab == null ||
            assets.DamageCanvas == null || assets.WorldCamera == null)
            return null;

        RectTransform canvasRect = assets.DamageCanvas;
        DamagePopup damagePopup = Instantiate(assets.DamagePopupPrefab, canvasRect);
        Vector2 screenPosition = assets.WorldCamera.WorldToScreenPoint(position);
        RectTransform popupRect = damagePopup.GetComponent<RectTransform>();
        Canvas canvas = canvasRect.GetComponentInParent<Canvas>();
        Camera uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera : null;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPosition, uiCamera, out Vector2 canvasPosition);

        popupRect.anchorMin = canvasRect.pivot;
        popupRect.anchorMax = canvasRect.pivot;
        popupRect.anchoredPosition = canvasPosition + damagePopup.screenOffset;

        damagePopup.ShowDamage(damageAmount);
        return damagePopup;
    }

    public void ShowDamage(int damageAmount)
    {
        if (textComponent != null)
        {
            textComponent.text = "-" + damageAmount.ToString() + " HP";
            colorText = textComponent.color;
            displayDuration = lifetime;
        }
    }

    private void Update()
    {
        if (textComponent == null)
            return;

        transform.position += new Vector3(0f, floatSpeed) * Time.deltaTime;

        displayDuration -= Time.deltaTime;

        if (displayDuration < 0)
        {
            colorText.a -= fadeSpeed * Time.deltaTime;
            textComponent.color = colorText;

            if (colorText.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
