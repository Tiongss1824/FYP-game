using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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

    // --- CHANGED: Split the events into two separate timers! ---
    [Header("Reward Event (Money / Items)")]
    [Tooltip("Seconds to wait before giving money (Time it with dialogue!)")]
    public float rewardDelay = 2.0f;
    public UnityEvent onRewardGiven;

    [Header("Final Cutscene Event (Fainting)")]
    [Tooltip("Seconds to wait before the cutscene triggers")]
    public float cutsceneDelay = 4.5f;
    public UnityEvent onCutsceneStart;

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
            if (postTaskConversations.Length > 0)
            {
                dialogueManager.StartDialogue(npcName, postTaskConversations[postTaskIndex].lines);
                if (postTaskIndex < postTaskConversations.Length - 1) postTaskIndex++;
            }

            if (!hasTriggeredEvent)
            {
                hasTriggeredEvent = true;
                // Start both timers independently!
                StartCoroutine(TriggerRewardWithDelay());
                StartCoroutine(TriggerCutsceneWithDelay());
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

    // Timer 1: Gives the money
    private IEnumerator TriggerRewardWithDelay()
    {
        yield return new WaitForSeconds(rewardDelay);
        onRewardGiven.Invoke();
    }

    // Timer 2: Starts the faint
    private IEnumerator TriggerCutsceneWithDelay()
    {
        yield return new WaitForSeconds(cutsceneDelay);
        onCutsceneStart.Invoke();
    }
}