using UnityEngine;

public class RingClue : MonoBehaviour, IInteractable
{
    [SerializeField] private NpcTalk alvinTalk;

    private bool ringFound = false;

    public string GetInteractPrompt()
    {
        return ringFound
            ? "Ring found — return to Alvin"
            : "Press [E] to inspect the ring";
    }

    public void OnInteract()
    {
        if (ringFound) return;

        ringFound = true;

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.MarkRingFound();
        }

        if (alvinTalk != null)
        {
            alvinTalk.CompleteTask();
        }
    }
}