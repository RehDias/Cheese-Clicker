using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textComponent;
    private float displayDuration;
    private Color colorText;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        DamagePopup damagePopup = Instantiate(GameAssets._i.DamagePopupPrefab, GameAssets._i.DamageCanvas);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);

        RectTransform canvasRect = GameAssets._i.DamageCanvas;
        RectTransform popupRect = damagePopup.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 canvasPosition);

        popupRect.anchorMin = canvasRect.pivot;
        popupRect.anchorMax = canvasRect.pivot;
        popupRect.anchoredPosition = canvasPosition + new Vector2(0f, 30f);

        damagePopup.ShowDamage(damageAmount);
        return damagePopup;
    }

    public void ShowDamage(int damageAmount)
    {
        if (textComponent != null)
        {
            textComponent.text = "-" + damageAmount.ToString() + " HP";
            colorText = textComponent.color;
            displayDuration = 1.3f;
        }
    }

    private void Update()
    {
        float floatSpeed = 20f;
        float disappearSpeed = 3;

        transform.position += new Vector3(0f, floatSpeed) * Time.deltaTime;

        displayDuration -= Time.deltaTime;

        if (displayDuration < 0)
        {
            colorText.a -= disappearSpeed * Time.deltaTime;
            textComponent.color = colorText;

            if (colorText.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
