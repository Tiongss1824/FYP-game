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

    public bool HasBeenAssigned { get; private set; } = false;

    [Header("Before Task Dialogue")]
    public Conversation[] preTaskConversations;
    private int preTaskIndex = 0;
    private bool hasTriggeredPreTaskEvent = false; // NEW

    [Header("After Task Dialogue")]
    public Conversation[] postTaskConversations;
    private int postTaskIndex = 0;

    [Header("Mini-Game Trigger Event")]
    [Tooltip("Fires exactly when the LAST pre-task dialogue box closes — wire this to whatever opens your minigame UI.")]
    public UnityEvent onReadyForMinigame; // NEW

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
            HasBeenAssigned = true;

            if (preTaskConversations.Length > 0)
            {
                int currentIndex = preTaskIndex; // capture BEFORE it changes

                dialogueManager.StartDialogue(npcName, preTaskConversations[currentIndex].lines);

                // NEW: if this is the last pre-task conversation, hook into
                // dialogue-finished so the minigame UI opens once the box closes.
                bool isLastPreTaskConvo = currentIndex == preTaskConversations.Length - 1;
                if (isLastPreTaskConvo && !hasTriggeredPreTaskEvent)
                {
                    hasTriggeredPreTaskEvent = true;
                    dialogueManager.onDialogueFinished += TriggerPreTaskEvents;
                }

                if (preTaskIndex < preTaskConversations.Length - 1) preTaskIndex++;
            }
        }
        else
        {
            if (!hasTriggeredEvent)
            {
                hasTriggeredEvent = true;

                dialogueManager.onDialogueFinished += TriggerQuestEvents;

                if (postTaskConversations.Length > 0)
                {
                    dialogueManager.StartDialogue(npcName, postTaskConversations[postTaskIndex].lines);
                    if (postTaskIndex < postTaskConversations.Length - 1) postTaskIndex++;
                }
                else
                {
                    TriggerQuestEvents();
                }
            }
            else
            {
                // NEW: reward already given once — every interaction after that
                // just reopens the minigame so the player can replay for fun.
                onReadyForMinigame.Invoke();
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

    // NEW: call this from your Cancel button so talking to the NPC again
    // will re-open the minigame instead of doing nothing.
    public void ResetMinigameTrigger()
    {
        hasTriggeredPreTaskEvent = false;
    }

    // NEW
    private void TriggerPreTaskEvents()
    {
        dialogueManager.onDialogueFinished -= TriggerPreTaskEvents; // unsubscribe so it only fires once
        onReadyForMinigame.Invoke();
    }

    private void TriggerQuestEvents()
    {
        dialogueManager.onDialogueFinished -= TriggerQuestEvents; // unsubscribe so it only fires once
        onQuestDialogueFinished.Invoke();
    }
}