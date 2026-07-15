using UnityEngine;

// Add ", IInteractable" to sign the contract!
public class MerchantNPC : MonoBehaviour, IInteractable
{
    [Header("Custom Prompt Text")]
    [Tooltip("Type exactly what you want the screen to say when looking at this NPC")]
    public string promptText = "Press [E] to Trade"; // <--- NEW

    // 1. Satisfy the contract by providing the text
    public string GetInteractPrompt()
    {
        return promptText; // <--- CHANGED: Now returns your custom string!
    }

    // 2. Satisfy the contract by providing the action
    public void OnInteract()
    {
        ShopManager.Instance.OpenShop();
    }
}