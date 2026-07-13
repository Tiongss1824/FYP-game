using UnityEngine;
using TMPro; // Required to talk to your UI text

public class WalletManager : MonoBehaviour
{
    // This creates a "Singleton" - it allows any other script in your game to easily find this wallet
    public static WalletManager Instance;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI cashTextUI; // Slot for your text object

    private int currentCash = 0;

    private void Awake()
    {
        // Set up the Singleton
        Instance = this;
    }

    private void Start()
    {
        // Update the text to $0 right when the game starts
        UpdateUI();
    }

    // You will trigger this function later when you complete a mini-game task
    public void AddCash(int amount)
    {
        currentCash += amount;
        UpdateUI();
    }

    // You will trigger this function later at the Pharmacy
    public bool TryBuyMedicine(int cost)
    {
        if (currentCash >= cost)
        {
            currentCash -= cost;
            UpdateUI();
            return true; // You had enough money!
        }
        else
        {
            Debug.Log("Not enough cash!");
            return false; // You are too broke!
        }
    }

    private void UpdateUI()
    {
        cashTextUI.text = "Cash: $ " + currentCash.ToString();
    }

    private void Update()
    {
        // Cheat code: Press 'M' to magically get $10
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddCash(10);
        }
    }
}