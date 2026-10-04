using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MerchantNpc : MonoBehaviour, IInteractable
{
    [Header("Merchant Settings")]
    public string npcName = "Merchant";

    [Tooltip("What the merchant says before the shop opens")]
    [TextArea(2, 5)]
    public string[] welcomeLines;

    private bool hasBeenIntroduced = false;

    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();
    }

    public string GetInteractPrompt()
    {
        return "Press [F] to Talk";
    }

    [Tooltip("What the merchant says after medicine bought.")]
    [TextArea(2, 5)]
    public string[] afterPurchaseLines;

    public void OnInteract()
    {
        // Medicine already bought -> new dialogue, no shop
        if (ShopManager.Instance != null && ShopManager.Instance.HasBoughtMedicine && afterPurchaseLines.Length > 0)
        {
            dialogueManager.StartDialogue(npcName, afterPurchaseLines);
            return;
        }

        if (!hasBeenIntroduced && welcomeLines.Length > 0)
        {
            hasBeenIntroduced = true;
            dialogueManager.onDialogueFinished += OpenShopAfterDialogue;
            dialogueManager.StartDialogue(npcName, welcomeLines);
        }
        else
        {
            OpenTheShopMenu();
        }
    }

    private void OpenShopAfterDialogue()
    {
        dialogueManager.onDialogueFinished -= OpenShopAfterDialogue;
        OpenTheShopMenu();
    }

    // Call this from other scripts (e.g. CinematicTrigger) once the NPC
    // has already been "met" through a cutscene, so the welcome line won't repeat later.
    public void MarkAsIntroduced()
    {
        hasBeenIntroduced = true;
    }

    private void OpenTheShopMenu()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShop();
        }
        else
        {
            Debug.LogError("ShopManager Instance is missing! Make sure ShopManager is in the scene.");
        }
    }
}