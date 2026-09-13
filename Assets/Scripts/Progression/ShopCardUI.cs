using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour
{
    [SerializeField] private ShopItemType itemType;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyButtonText;

    private ShopController controller;

    public ShopItemType ItemType => itemType;

    private void Awake()
    {
        FindChildReferences();
        if (buyButton != null)
            buyButton.onClick.AddListener(Buy);
    }

    private void OnDestroy()
    {
        if (buyButton != null)
            buyButton.onClick.RemoveListener(Buy);
    }

    public void Initialize(ShopController shopController)
    {
        controller = shopController;
        Refresh();
    }

    public void SetItemType(ShopItemType newItemType)
    {
        itemType = newItemType;
    }

    public void Refresh()
    {
        if (controller == null)
            return;

        ShopCardState state = controller.GetCardState(itemType);
        titleText.text = state.Title;
        descriptionText.text = state.Description;
        levelText.text = state.Status;
        priceText.text = state.IsComplete ? "--" : state.Price.ToString("N0");
        buyButtonText.text = state.IsComplete ? "OWNED" : "BUY";
        buyButton.interactable = !state.IsComplete && controller.CanAfford(state.Price);

        if (state.Icon != null)
            iconImage.sprite = state.Icon;
    }

    private void Buy()
    {
        if (controller != null)
            controller.TryBuy(itemType);
    }

    private void FindChildReferences()
    {
        titleText ??= FindChild<TMP_Text>("CardTitle");
        iconImage ??= FindChild<Image>("CardIcon");
        descriptionText ??= FindChild<TMP_Text>("Description");
        levelText ??= FindChild<TMP_Text>("LevelText");
        priceText ??= FindChild<TMP_Text>("PriceText");
        buyButton ??= FindChild<Button>("BuyButton");

        if (buyButtonText == null && buyButton != null)
            buyButtonText = buyButton.GetComponentInChildren<TMP_Text>(true);
    }

    private T FindChild<T>(string childName) where T : Component
    {
        foreach (T component in GetComponentsInChildren<T>(true))
        {
            if (component.name == childName)
                return component;
        }

        return null;
    }
}

public readonly struct ShopCardState
{
    public readonly string Title;
    public readonly string Description;
    public readonly string Status;
    public readonly long Price;
    public readonly bool IsComplete;
    public readonly Sprite Icon;

    public ShopCardState(string title, string description, string status, long price, bool isComplete, Sprite icon)
    {
        Title = title;
        Description = description;
        Status = status;
        Price = price;
        IsComplete = isComplete;
        Icon = icon;
    }
}
