using UnityEngine;

// Attach this to the physical object in the world the player interacts with
// to open the wiring puzzle (e.g. a fuse box or wall panel).
public class WirePanelInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Same NPC reference as TaskManager's Task 2 Talk / BookSortingManager's Task 2 Talk")]
    public NpcTalk task2Talk;

    [Header("Prompts")]
    public string notAssignedPrompt = "Nothing to do here";
    public string activePrompt = "Press [F] to Fix Wiring";
    public string completedPrompt = "The wiring is fixed";

    public string GetInteractPrompt()
    {
        if (task2Talk == null) return activePrompt;

        if (task2Talk.isTaskCompleted) return completedPrompt;
        if (!task2Talk.HasBeenAssigned) return notAssignedPrompt;

        return activePrompt;
    }

    public void OnInteract()
    {
        if (task2Talk == null) return;

        // Don't allow opening it before the NPC has actually assigned the task,
        // or after it's already been completed
        if (!task2Talk.HasBeenAssigned || task2Talk.isTaskCompleted) return;

        if (BookSortingManager.Instance != null)
        {
            BookSortingManager.Instance.OpenSortingUI();
        }
    }
}