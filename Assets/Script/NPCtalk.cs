using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Conversation
{
    [TextArea(2, 5)]
    public string[] lines;
}

public class NpcTalk : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public string npcName = "Pinut";

    [Header("Custom UI Prompts")]
    public string defaultPrompt = "Press [F] to Talk";
    public string readyToTurnInPrompt = "Press [F] to Complete Quest";

    [Header("Task Settings")]
    public bool isTaskCompleted = false;
    private bool hasTriggeredEvent = false;

    [Header("Before Task Dialogue")]
    public Conversation[] preTaskConversations;
    private int preTaskIndex = 0;

    [Header("After Task Dialogue")]
    public Conversation[] postTaskConversations;
    private int postTaskIndex = 0;

    [Header("Quest Completion Events")]
    [Tooltip("Fires exactly when the post-task dialogue box closes")]
    public UnityEvent onQuestDialogueFinished;

    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();
    }

    public string GetInteractPrompt()
    {
        return (isTaskCompleted && !hasTriggeredEvent) ? readyToTurnInPrompt : defaultPrompt;
    }

    public void OnInteract()
    {
        if (!isTaskCompleted)
        {
            if (preTaskConversations.Length > 0)
            {
                dialogueManager.StartDialogue(npcName, preTaskConversations[preTaskIndex].lines);
                if (preTaskIndex < preTaskConversations.Length - 1) preTaskIndex++;
            }
        }
        else
        {
            if (!hasTriggeredEvent)
            {
                hasTriggeredEvent = true;

                // 1. Tell DialogueManager: "When this text finishes, fire my generic event!"
                dialogueManager.onDialogueFinished += TriggerQuestEvents;

                // 2. Start the dialogue
                if (postTaskConversations.Length > 0)
                {
                    dialogueManager.StartDialogue(npcName, postTaskConversations[postTaskIndex].lines);
                    if (postTaskIndex < postTaskConversations.Length - 1) postTaskIndex++;
                }
                else
                {
                    TriggerQuestEvents(); // Failsafe if there is no text
                }
            }
        }
    }

    public void Interact()
    {
        OnInteract();
    }

    public void CompleteTask()
    {
        isTaskCompleted = true;
    }

    private void TriggerQuestEvents()
    {
        onQuestDialogueFinished.Invoke();
    }
}