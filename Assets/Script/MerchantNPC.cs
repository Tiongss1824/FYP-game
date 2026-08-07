using UnityEngine;
using System.Collections;

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

    public void OnInteract()
    {
        if (welcomeLines.Length > 0)
        {
            // 1. Tell the DialogueManager: "When you finish this text, run my OpenTheShopMenu function!"
            dialogueManager.onDialogueFinished += OpenTheShopMenu;

            // 2. Start the dialogue
            dialogueManager.StartDialogue(npcName, welcomeLines);
        }
        else
        {
            // If the merchant has no text setup, just open the shop instantly
            OpenTheShopMenu();
        }
    }

    // NEW: call this from other scripts (e.g. CinematicTrigger) once the NPC
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