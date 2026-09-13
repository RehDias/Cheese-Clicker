using TMPro;
using UnityEngine;

public class CheeseWalletDisplay : MonoBehaviour
{
    [SerializeField] private CheeseWallet wallet;
    [SerializeField] private TMP_Text balanceText;

    private void Awake()
    {
        if (wallet == null)
            wallet = FindAnyObjectByType<CheeseWallet>();
        if (balanceText == null)
            balanceText = GetComponent<TMP_Text>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForCoinText()
    {
        foreach (TMP_Text text in FindObjectsByType<TMP_Text>(FindObjectsInactive.Include))
        {
            if (text.name == "CoinText" && text.GetComponent<CheeseWalletDisplay>() == null)
            {
                text.gameObject.AddComponent<CheeseWalletDisplay>();
                return;
            }
        }
    }

    private void OnEnable()
    {
        if (wallet == null)
            return;

        wallet.BalanceChanged += UpdateText;
        UpdateText(wallet.Balance);
    }

    private void OnDisable()
    {
        if (wallet != null)
            wallet.BalanceChanged -= UpdateText;
    }

    private void UpdateText(long balance)
    {
        if (balanceText != null)
            balanceText.text = balance.ToString("N0");
    }
}
