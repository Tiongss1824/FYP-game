using UnityEngine;

// Add ", IInteractable" to sign the contract!
public class MerchantNPC : MonoBehaviour, IInteractable
{
    // 1. Satisfy the contract by providing the text
    public string GetInteractPrompt()
    {
        return "Press [E] to Trade";
    }

    // 2. Satisfy the contract by providing the action
    public void OnInteract()
    {
        ShopManager.Instance.OpenShop();
    }
}