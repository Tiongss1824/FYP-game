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
    [Tooltip("How many correct placements are needed to finish the task")]
    public int totalBooksRequired = 5;

    private int correctlyPlacedCount = 0;
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

        correctlyPlacedCount = 0;

        BookDrag[] allPieces = sortingUIPanel.GetComponentsInChildren<BookDrag>(true);
        foreach (BookDrag piece in allPieces)
        {
            piece.ResetToStart();
        }

        BookSlot[] allSlots = sortingUIPanel.GetComponentsInChildren<BookSlot>(true);
        foreach (BookSlot slot in allSlots)
        {
            slot.ResetSlot();
        }

        CloseSortingUI();
    }

    // Called by BookSlot every time a piece is placed correctly
    public void OnBookPlacedCorrectly()
    {
        if (isComplete) return;

        correctlyPlacedCount++;

        if (correctlyPlacedCount >= totalBooksRequired)
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

        // Small delay so the player can see the last piece snap in before the panel closes
        Invoke(nameof(CloseSortingUI), 0.6f);
    }
}