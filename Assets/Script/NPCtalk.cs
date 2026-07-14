using UnityEngine;

[System.Serializable]
public class Conversation
{
    [TextArea(2, 5)]
    public string[] lines;
}

public class NPCtalk : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public string npcName = "Pinut";

    [Header("Standard Dialogue")]
    [Tooltip("Add multiple conversations here. The NPC will cycle through them.")]
    public Conversation[] standardConversations;

    private int currentConvoIndex = 0;
    private DialogueManager dialogueManager;

    private void Start()
    {
        // Find the manager ONCE when the game boots up.
        dialogueManager = FindAnyObjectByType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("No DialogueManager found in the scene! Did you forget to add it?");
        }
    }

    public void Interact()
    {
        // Safety check: If we forgot to write dialogue in the editor, don't crash.
        if (standardConversations.Length == 0) return;

        // 1. Pick the current conversation
        Conversation convoToPlay = standardConversations[currentConvoIndex];

        // 2. Send it to the screen
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(npcName, convoToPlay.lines);
        }

        // 3. Move to the next conversation for the NEXT time we talk to them
        // (But stop at the very last one so they don't run out of things to say!)
        if (currentConvoIndex < standardConversations.Length - 1)
        {
            currentConvoIndex++;
        }
    }

    public string GetInteractPrompt()
    {
        return "Press [F] to Talk";
    }

    public void OnInteract()
    {
        Interact();
    }
}