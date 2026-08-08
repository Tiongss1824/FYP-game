using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach this to each shelf slot. Set "requiredColor" to match the book
// (by color, via BookDraggable's own Image) that belongs here.
public class BookSlot : MonoBehaviour, IDropHandler
{
    [Tooltip("The book color that belongs in this slot")]
    public Color requiredColor = Color.white;

    [Tooltip("How close a color needs to be to count as a match (helps with tiny rounding differences)")]
    public float colorTolerance = 0.05f;

    private bool isFilled = false;

    // Called by BookSortingManager when the player closes the puzzle early
    public void ResetSlot()
    {
        isFilled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isFilled) return; // already has a correct book, ignore further drops

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        BookDrag book = droppedObject.GetComponent<BookDrag>();
        Image bookImage = droppedObject.GetComponent<Image>();

        if (book == null || bookImage == null) return;

        if (ColorsMatch(bookImage.color, requiredColor))
        {
            RectTransform slotRect = GetComponent<RectTransform>();
            book.SnapInto(slotRect);
            isFilled = true;

            // Let the overall sorting manager know progress happened
            if (BookSortingManager.Instance != null)
            {
                BookSortingManager.Instance.OnBookPlacedCorrectly();
            }
        }
        // If it doesn't match, do nothing here — BookDraggable's OnEndDrag
        // will detect it wasn't accepted and snap it back to its start position.
    }

    private bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < colorTolerance
            && Mathf.Abs(a.g - b.g) < colorTolerance
            && Mathf.Abs(a.b - b.b) < colorTolerance;
    }
}