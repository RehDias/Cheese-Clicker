using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    [Header("Economy")]
    [SerializeField] private CheeseWallet wallet;
    [SerializeField] private PlayerProgress progress;

    [Header("Shop UI")]
    [SerializeField] private GameObject shopOverlay;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform cardsContainer;

    [Header("Characters")]
    [SerializeField] private CharacterShooter spear;
    [SerializeField] private CharacterShooter shooter;
    [SerializeField] private CharacterShooter mage;

    [Header("Card Icons")]
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private Sprite automaticIcon;
    [SerializeField] private Sprite shooterIcon;
    [SerializeField] private Sprite mageIcon;

    [Header("Prices")]
    [SerializeField] private long[] speedPrices = { 25, 50, 100 };
    [SerializeField] private long[] damagePrices = { 50, 100, 200, 350, 550, 800 };
    [SerializeField, Min(0)] private long automaticAttackPrice = 150;
    [SerializeField, Min(0)] private long shooterPrice = 250;
    [SerializeField, Min(0)] private long magePrice = 500;

    private ShopCardUI[] cards;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForCurrentScene()
    {
        if (FindAnyObjectByType<ShopController>() != null)
            return;

        CheeseWallet sceneWallet = FindAnyObjectByType<CheeseWallet>();
        if (sceneWallet != null)
            sceneWallet.gameObject.AddComponent<ShopController>();
    }

    private void Awake()
    {
        FindSceneReferences();
        CreateMissingCards();

        cards = cardsContainer != null
            ? cardsContainer.GetComponentsInChildren<ShopCardUI>(true)
            : GetComponentsInChildren<ShopCardUI>(true);

        foreach (ShopCardUI card in cards)
            card.Initialize(this);

        if (openButton != null)
            openButton.onClick.AddListener(OpenShop);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);

        ApplyOwnedUnlocks();
        CloseShop();
    }

    private void OnEnable()
    {
        if (wallet != null)
            wallet.BalanceChanged += HandleBalanceChanged;
        if (progress != null)
            progress.ProgressChanged += RefreshCards;
    }

    private void OnDisable()
    {
        if (wallet != null)
            wallet.BalanceChanged -= HandleBalanceChanged;
        if (progress != null)
            progress.ProgressChanged -= RefreshCards;
    }

    private void OnDestroy()
    {
        if (openButton != null)
            openButton.onClick.RemoveListener(OpenShop);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseShop);
    }

    public bool CanAfford(long price)
    {
        return wallet != null && wallet.CanAfford(price);
    }

    public void TryBuy(ShopItemType itemType)
    {
        ShopCardState state = GetCardState(itemType);
        if (state.IsComplete || wallet == null || !wallet.TrySpend(state.Price))
            return;

        bool purchaseApplied = ApplyPurchase(itemType);
        if (!purchaseApplied)
        {
            wallet.AddCoins(state.Price);
            return;
        }

        ApplyOwnedUnlocks();
        RefreshCards();
    }

    public ShopCardState GetCardState(ShopItemType itemType)
    {
        if (progress == null)
            return new ShopCardState("NOT READY", "PlayerProgress is missing.", "", 0, true, null);

        switch (itemType)
        {
            case ShopItemType.AttackSpeed:
                return LevelCard(
                    "ATTACK SPEED", "Attacks animate and land 20% faster per level.",
                    progress.AttackSpeedLevel, PlayerProgress.MaxAttackSpeedLevel,
                    speedPrices, speedIcon);
            case ShopItemType.AttackDamage:
                return LevelCard(
                    "ATTACK DAMAGE", "Every character deals 25% more damage per level.",
                    progress.AttackDamageLevel, PlayerProgress.MaxAttackDamageLevel,
                    damagePrices, damageIcon);
            case ShopItemType.AutomaticAttack:
                return UnlockCard(
                    "AUTO ATTACK", "Unlocked characters attack living cheese automatically.",
                    automaticAttackPrice, progress.AutomaticAttackUnlocked, automaticIcon);
            case ShopItemType.Shooter:
                return UnlockCard(
                    "SHOOTER", "Unlock the ranged character and cheese projectile.",
                    shooterPrice, progress.ShooterUnlocked, shooterIcon);
            case ShopItemType.Mage:
                return UnlockCard(
                    "MAGE", "Unlock the mage as another automatic attacker.",
                    magePrice, progress.MageUnlocked, mageIcon);
            default:
                return new ShopCardState("UNKNOWN", "", "", 0, true, null);
        }
    }

    public void OpenShop()
    {
        if (shopOverlay != null)
            shopOverlay.SetActive(true);
        RefreshCards();
    }

    public void CloseShop()
    {
        if (shopOverlay != null)
            shopOverlay.SetActive(false);
    }

    private bool ApplyPurchase(ShopItemType itemType)
    {
        switch (itemType)
        {
            case ShopItemType.AttackSpeed:
                return progress.TryAddAttackSpeedLevel();
            case ShopItemType.AttackDamage:
                return progress.TryAddAttackDamageLevel();
            case ShopItemType.AutomaticAttack:
                return progress.TryUnlockAutomaticAttack();
            case ShopItemType.Shooter:
                return progress.TryUnlockShooter();
            case ShopItemType.Mage:
                return progress.TryUnlockMage();
            default:
                return false;
        }
    }

    private void ApplyOwnedUnlocks()
    {
        if (progress == null)
            return;

        if (spear != null)
            spear.gameObject.SetActive(true);
        SetCharacterUnlocked(shooter, progress.ShooterUnlocked);
        SetCharacterUnlocked(mage, progress.MageUnlocked);

        if (progress.AutomaticAttackUnlocked)
        {
            UnlockAutomaticAttack(spear);
            UnlockAutomaticAttack(shooter);
            UnlockAutomaticAttack(mage);
        }

        if (progress.MageUnlocked && mage != null)
            mage.MakeActiveManualAttacker();
        else if (progress.ShooterUnlocked && shooter != null)
            shooter.MakeActiveManualAttacker();
        else if (spear != null)
            spear.MakeActiveManualAttacker();
    }

    private void SetCharacterUnlocked(CharacterShooter character, bool unlocked)
    {
        if (character == null)
            return;

        character.gameObject.SetActive(unlocked);
        if (unlocked && progress.AutomaticAttackUnlocked)
            character.UnlockAutomaticAttack();
    }

    private static void UnlockAutomaticAttack(CharacterShooter character)
    {
        if (character != null && character.gameObject.activeInHierarchy)
            character.UnlockAutomaticAttack();
    }

    private static ShopCardState LevelCard(
        string title, string description, int level, int maxLevel, long[] prices, Sprite icon)
    {
        bool complete = level >= maxLevel;
        int priceIndex = Mathf.Clamp(level, 0, Mathf.Max(0, prices.Length - 1));
        long price = prices.Length == 0 ? 0 : prices[priceIndex];
        return new ShopCardState(title, description, $"LEVEL {level}/{maxLevel}", price, complete, icon);
    }

    private static ShopCardState UnlockCard(
        string title, string description, long price, bool unlocked, Sprite icon)
    {
        return new ShopCardState(title, description, unlocked ? "UNLOCKED" : "LOCKED", price, unlocked, icon);
    }

    private void HandleBalanceChanged(long unusedBalance)
    {
        RefreshCards();
    }

    private void RefreshCards()
    {
        if (cards == null)
            return;

        foreach (ShopCardUI card in cards)
            card.Refresh();
    }

    private void FindSceneReferences()
    {
        wallet ??= FindAnyObjectByType<CheeseWallet>();
        progress ??= FindAnyObjectByType<PlayerProgress>();
        shopOverlay ??= FindSceneObject<GameObject>("ShopOverlay");
        cardsContainer ??= FindSceneObject<Transform>("CardsContainer");
        openButton ??= FindSceneObject<Button>("ShopButton");
        closeButton ??= FindSceneObject<Button>("CloseButton");
        spear ??= FindSceneObject<CharacterShooter>("Spear");
        shooter ??= FindSceneObject<CharacterShooter>("Shooter");
        mage ??= FindSceneObject<CharacterShooter>("Mage");

        GameAssets assets = GameAssets.Instance;
        if (assets != null)
        {
            speedIcon ??= assets.AttackSpeedIcon;
            damageIcon ??= assets.AttackDamageIcon;
            automaticIcon ??= assets.CheeseCoinIcon;
        }

        if (shooterIcon == null && shooter != null)
            shooterIcon = shooter.GetComponent<SpriteRenderer>().sprite;
        if (mageIcon == null && mage != null)
            mageIcon = mage.GetComponent<SpriteRenderer>().sprite;
    }

    private void CreateMissingCards()
    {
        if (cardsContainer == null)
            return;

        ShopCardUI template = null;
        foreach (Transform child in cardsContainer)
        {
            if (child.name == "SpeedCard")
            {
                template = child.GetComponent<ShopCardUI>();
                if (template == null)
                    template = child.gameObject.AddComponent<ShopCardUI>();
                template.SetItemType(ShopItemType.AttackSpeed);
                break;
            }
        }

        if (template == null)
        {
            Debug.LogError("CardsContainer needs the SpeedCard template.", this);
            return;
        }

        ArrangeCard(template.transform);
        CreateCardIfMissing(template, "DamageCard", ShopItemType.AttackDamage);
        CreateCardIfMissing(template, "AutomaticCard", ShopItemType.AutomaticAttack);
        CreateCardIfMissing(template, "ShooterCard", ShopItemType.Shooter);
        CreateCardIfMissing(template, "MageCard", ShopItemType.Mage);

        HorizontalLayoutGroup layout = cardsContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            layout.spacing = 28f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        RectTransform content = cardsContainer as RectTransform;
        if (content != null)
        {
            content.anchorMin = new Vector2(0f, 0.5f);
            content.anchorMax = new Vector2(0f, 0.5f);
            content.pivot = new Vector2(0f, 0.5f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(content.sizeDelta.x, 500f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }

    private void CreateCardIfMissing(ShopCardUI template, string cardName, ShopItemType itemType)
    {
        foreach (Transform child in cardsContainer)
        {
            if (child.name == cardName)
                return;
        }

        ShopCardUI card = Instantiate(template, cardsContainer);
        card.name = cardName;
        card.SetItemType(itemType);
        ArrangeCard(card.transform);
    }

    private static void ArrangeCard(Transform card)
    {
        RectTransform cardRect = card as RectTransform;
        if (cardRect != null)
            cardRect.sizeDelta = new Vector2(280f, 396f);

        LayoutElement layoutElement = card.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.preferredWidth = 280f;
            layoutElement.preferredHeight = 396f;
        }

        Image background = card.GetComponent<Image>();
        if (background != null)
            background.preserveAspect = true;

        SetRect(card.Find("CardTitle") as RectTransform, new Vector2(0f, 140f), new Vector2(240f, 55f));
        SetRect(card.Find("CardIcon") as RectTransform, new Vector2(0f, 55f), new Vector2(100f, 100f));
        SetRect(card.Find("Description") as RectTransform, new Vector2(0f, -25f), new Vector2(220f, 72f));
        SetRect(card.Find("LevelText") as RectTransform, new Vector2(0f, -85f), new Vector2(210f, 36f));
        Transform priceContainer = card.Find("PriceContainer");
        SetRect(priceContainer as RectTransform, new Vector2(8f, -220f), new Vector2(140f, 42f));
        SetRect(priceContainer?.Find("PriceCoinIcon") as RectTransform, Vector2.zero, new Vector2(30f, 30f));
        SetRect(priceContainer?.Find("PriceText") as RectTransform, Vector2.zero, new Vector2(95f, 42f));

        TMP_Text priceText = priceContainer != null
            ? priceContainer.Find("PriceText")?.GetComponent<TMP_Text>()
            : null;
        if (priceText != null)
            priceText.alignment = TextAlignmentOptions.Center;

        HorizontalLayoutGroup priceLayout = priceContainer != null
            ? priceContainer.GetComponent<HorizontalLayoutGroup>()
            : null;
        if (priceLayout != null)
        {
            priceLayout.padding = new RectOffset(0, 0, 0, 0);
            priceLayout.spacing = 6f;
            priceLayout.childAlignment = TextAnchor.MiddleCenter;
            priceLayout.childControlWidth = false;
            priceLayout.childControlHeight = false;
            priceLayout.childForceExpandWidth = false;
            priceLayout.childForceExpandHeight = false;
        }

        SetRect(card.Find("BuyButton") as RectTransform, new Vector2(0f, -170f), new Vector2(125f, 48f));
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        if (rect == null)
            return;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static T FindSceneObject<T>(string objectName) where T : Object
    {
        foreach (T candidate in Resources.FindObjectsOfTypeAll<T>())
        {
            GameObject gameObject = null;
            if (candidate is GameObject candidateObject)
                gameObject = candidateObject;
            else if (candidate is Component component)
                gameObject = component.gameObject;

            if (gameObject != null && gameObject.scene.IsValid() && gameObject.name == objectName)
                return candidate;
        }

        return null;
    }
}
