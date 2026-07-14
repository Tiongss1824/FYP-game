using UnityEngine;
using StarterAssets; // Needed to freeze your FirstPersonController

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI References")]
    public GameObject shopUIPanel; // Drag your ShopMenu_UI here

    [Header("Player References")]
    public FirstPersonController playerController;

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

        // Lock and hide the mouse cursor for first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 3. The actual purchase logic
    public void BuyMedicine()
    {
        int medicinePrice = 150;

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
    }
}