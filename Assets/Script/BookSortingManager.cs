using UnityEngine;
using StarterAssets;
using UnityEngine.EventSystems;

public class BookSortingManager : MonoBehaviour
{
    public static BookSortingManager Instance;

    [Header("UI References")]
    public GameObject sortingUIPanel;

    [Header("Player References")]
    public FirstPersonController playerController;

    [Header("Task 2 NPC")]
    [Tooltip("Drag the task 2 NPC's GameObject (the one with NpcTalk on it) — same reference as TaskManager's Task 2 Talk")]
    public NpcTalk task2Talk;

    [Header("Puzzle Settings")]
    [Tooltip("Should equal the total number of slots — the puzzle completes when every slot has its correct color")]
    public int totalBooksRequired = 12;

    [Header("Completion Cutscene")]
    [Tooltip("Drag NPC2's actual GameObject/model in the scene (the one that moves around)")]
    public Transform npcToMove;

    [Tooltip("An empty GameObject placed wherever you want NPC2 to appear")]
    public Transform arrivalPoint;

    [Tooltip("The CinematicTrigger already set up with NPC2 as its npcCharacter — no collider needed since this calls it manually")]
    public CinematicTrigger cutsceneTrigger;

    private bool isComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (sortingUIPanel != null) sortingUIPanel.SetActive(false);
    }

    // Call this from your interactable object (e.g. the fuse box) when the player presses F
    public void OpenSortingUI()
    {
        sortingUIPanel.SetActive(true);

        if (playerController != null) playerController.enabled = false;

        StarterAssetsInputs starterInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (starterInputs != null)
        {
            starterInputs.cursorLocked = false;
            starterInputs.cursorInputForLook = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseSortingUI()
    {
        sortingUIPanel.SetActive(false);

        if (playerController != null) playerController.enabled = true;

        StarterAssetsInputs starterInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (starterInputs != null)
        {
            starterInputs.cursorLocked = true;
            starterInputs.cursorInputForLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Wire this to your Close/Cancel button's OnClick(). If the puzzle isn't
    // finished yet, this resets ALL progress as a small punishment for quitting early.
    public void CancelPuzzle()
    {
        if (isComplete)
        {
            // Already done — just let them close it, no punishment
            CloseSortingUI();
            return;
        }

        BookDrag[] allPieces = sortingUIPanel.GetComponentsInChildren<BookDrag>(true);
        foreach (BookDrag piece in allPieces)
        {
            if (piece.IsClone)
            {
                // Clones came from an infinite-supply palette piece — just remove them
                Destroy(piece.gameObject);
            }
            else
            {
                // Palette source pieces never actually move themselves, but reset just in case
                piece.ResetToStart();
            }
        }

        BookSlot[] allSlots = sortingUIPanel.GetComponentsInChildren<BookSlot>(true);
        foreach (BookSlot slot in allSlots)
        {
            slot.ResetSlot();
        }

        CloseSortingUI();
    }

    // Called by BookSlot every time ANY piece is dropped into a slot (right or wrong color).
    // Rechecks every slot from scratch, since pieces can now be swapped around freely.
    public void CheckCompletion()
    {
        if (isComplete) return;

        BookSlot[] allSlots = sortingUIPanel.GetComponentsInChildren<BookSlot>(true);

        int correctCount = 0;
        foreach (BookSlot slot in allSlots)
        {
            if (slot.IsCorrect()) correctCount++;
        }

        if (correctCount >= totalBooksRequired && correctCount >= allSlots.Length)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        isComplete = true;

        // Mark the quest complete on the NPC, same pattern CountVege uses for the old man
        if (task2Talk != null)
        {
            task2Talk.CompleteTask();
        }

        // Teleport NPC2 to the arrival point, then play the camera-turn-and-talk cutscene
        // — same direct-call pattern as CountVege.CompleteQuest() uses for the old man
        if (npcToMove != null && arrivalPoint != null)
        {
            npcToMove.position = arrivalPoint.position;
            npcToMove.rotation = arrivalPoint.rotation;
        }

        if (cutsceneTrigger != null)
        {
            cutsceneTrigger.StartCutsceneManually();
        }

        // Small delay so the player can see the last piece snap in before the panel closes
        Invoke(nameof(CloseSortingUI), 0.6f);
    }
}