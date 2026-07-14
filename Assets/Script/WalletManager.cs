using UnityEngine;
using TMPro;

public class WalletManager : MonoBehaviour
{
    // The Singleton - makes this script globally accessible
    public static WalletManager Instance;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI cashTextUI;

    private int currentCash = 0;

    private void Awake()
    {
        // Set up the Singleton when the game boots
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevents accidental duplicates
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCash(int amount)
    {
        currentCash += amount;
        UpdateUI();
    }

    public bool TryBuyMedicine(int cost)
    {
        if (currentCash >= cost)
        {
            currentCash -= cost;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough cash!");
            return false;
        }
    }

    private void UpdateUI()
    {
        if (cashTextUI != null)
        {
            cashTextUI.text = "Cash: $ " + currentCash.ToString();
        }
    }
}