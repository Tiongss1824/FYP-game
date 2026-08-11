using UnityEngine;
using StarterAssets;
using UnityEngine.EventSystems; // Required for the sticky button fix!

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI References")]
    public GameObject shopUIPanel;

    [Header("Player References")]
    public FirstPersonController playerController;

    [Header("Purchase Settings")]
    public int medicinePrice = 150;

    [Header("Not Enough Money Hint")]
    [Tooltip("Drag your DialogueManager here")]
    public DialogueManager dialogueManager;

    [Tooltip("The name shown in the dialogue box for this hint")]
    public string merchantName = "Merchant";

    [Tooltip("What the merchant says when you leave without enough money for medicine")]
    [TextArea(2, 5)]
    public string[] notEnoughMoneyLines;

    private void Awake()
    {
        Instance = this;
    }

    // 1. Opens the shop and gives you your mouse back
    public void OpenShop()
    {
        shopUIPanel.SetActive(true);

        // Freeze movement
        playerController.enabled = false;

        // Tell StarterAssets to release the mouse and stop looking around
        StarterAssetsInputs starterInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (starterInputs != null)
        {
            starterInputs.cursorLocked = false;
            starterInputs.cursorInputForLook = false;
        }

        // Unlock and show the mouse cursor so you can click buttons!
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 2. Closes the shop and hides the mouse again
    public void CloseShop()
    {
        shopUIPanel.SetActive(false);

        // Unfreeze movement
        playerController.enabled = true;

        // Tell StarterAssets to lock the mouse again and resume looking
        StarterAssetsInputs starterInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (starterInputs != null)
        {
            starterInputs.cursorLocked = true;
            starterInputs.cursorInputForLook = true;
        }

        // Lock and hide the mouse cursor for first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Clear the selected button so "Close" doesn't stay highlighted next time you open the shop
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // NEW: wire this to your "Leave" button instead of CloseShop() directly.
    // If the player can't afford the medicine yet, the merchant gives a hint before closing.
    public void LeaveShop()
    {
        bool canAffordMedicine = WalletManager.Instance != null && WalletManager.Instance.CurrentCash >= medicinePrice;

        if (!canAffordMedicine && notEnoughMoneyLines.Length > 0 && dialogueManager != null)
        {
            CloseShop();
            dialogueManager.StartDialogue(merchantName, notEnoughMoneyLines);
        }
        else
        {
            CloseShop();
        }
    }

    // 3. The actual purchase logic
    public void BuyMedicine()
    {
        // Check with your WalletManager!
        if (WalletManager.Instance.TryBuyMedicine(medicinePrice))
        {
            Debug.Log("Successfully bought Medicine! Money deducted.");

            // TODO: Add boolean here like 'hasMedicine = true;' for your final quest
        }
        else
        {
            Debug.Log("Not enough cash!");
            // Optional: You could make a UI text flash red here saying "Not enough money"
        }

        // Clear the selected button so "Buy" doesn't stay highlighted!
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}