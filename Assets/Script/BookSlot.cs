using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach this to each shelf/socket slot. Every slot now accepts ANY color piece —
// "requiredColor" is only used to check correctness, not to block the drop.
public class BookSlot : MonoBehaviour, IDropHandler
{
    [Tooltip("The color that's actually correct for this slot")]
    public Color requiredColor = Color.white;

    [Tooltip("How close a color needs to be to count as a match (helps with tiny rounding differences)")]
    public float colorTolerance = 0.05f;

    private BookDrag currentBook; // whatever's currently sitting here, right or wrong

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        BookDrag book = droppedObject.GetComponent<BookDrag>();
        if (book == null) return;

        // If this slot already has something in it, kick that piece back to its pile first
        if (currentBook != null && currentBook != book)
        {
            currentBook.ReturnToStart();
        }

        RectTransform slotRect = GetComponent<RectTransform>();
        book.SnapInto(slotRect, this);
        currentBook = book;

        if (BookSortingManager.Instance != null)
        {
            BookSortingManager.Instance.CheckCompletion();
        }
    }

    // Called by BookDraggable when the piece here is picked back up
    public void ClearSlot()
    {
        currentBook = null;
    }

    // Called by BookSortingManager when the player cancels early
    public void ResetSlot()
    {
        currentBook = null;
    }

    // Used by BookSortingManager to check if this slot currently holds the right color
    public bool IsCorrect()
    {
        if (currentBook == null) return false;

        Image bookImage = currentBook.GetComponent<Image>();
        if (bookImage == null) return false;

        return ColorsMatch(bookImage.color, requiredColor);
    }

    private bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < colorTolerance
            && Mathf.Abs(a.g - b.g) < colorTolerance
            && Mathf.Abs(a.b - b.b) < colorTolerance;
    }
}