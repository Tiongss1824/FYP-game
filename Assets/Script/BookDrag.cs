using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach this to each draggable book/wire UI Image.
// The book's own Image color IS its "color tag" — BookSlot compares against it directly.
[RequireComponent(typeof(CanvasGroup))]
public class BookDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Tick this for a reusable palette piece (e.g. one wire color that fills multiple slots). Dragging it spawns a copy; the original stays put.")]
    public bool isSourcePalette = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private Vector2 originalAnchoredPosition;
    private Transform originalParent;

    // The slot this piece is currently sitting in, if any (right or wrong color — doesn't matter here)
    private BookSlot currentSlot;

    // True for pieces spawned from a palette source, so Cancel can destroy them instead of resetting them
    public bool IsClone { get; private set; } = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();

        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSourcePalette)
        {
            SpawnCloneAndBeginDrag(eventData);
            return;
        }

        // If this piece was sitting in a slot, free that slot up first
        if (currentSlot != null)
        {
            currentSlot.ClearSlot();
            currentSlot = null;
        }

        transform.SetParent(rootCanvas.transform, true);
        canvasGroup.blocksRaycasts = false; // lets BookSlot detect the drop underneath the piece
    }

    private void SpawnCloneAndBeginDrag(PointerEventData eventData)
    {
        GameObject clone = Instantiate(gameObject, transform.parent);
        BookDrag cloneDraggable = clone.GetComponent<BookDrag>();
        cloneDraggable.isSourcePalette = false; // the clone behaves as a normal, single-use piece
        cloneDraggable.MarkAsClone();

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        cloneRect.anchoredPosition = rectTransform.anchoredPosition;

        // Hand the drag off to the newly spawned clone instead of the source piece
        eventData.pointerDrag = clone;
        ExecuteEvents.Execute(clone, eventData, ExecuteEvents.beginDragHandler);
    }

    public void MarkAsClone()
    {
        IsClone = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // If nothing accepted the drop (BookSlot didn't call SnapInto), return to start
        if (transform.parent == rootCanvas.transform)
        {
            ReturnToStart();
        }
    }

    // Called by BookSlot whenever this piece is dropped into it — accepts any color
    public void SnapInto(RectTransform slot, BookSlot slotScript)
    {
        currentSlot = slotScript;
        transform.SetParent(slot, true);
        rectTransform.anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = true; // stays draggable so it can be picked back up
    }

    // Called by BookSlot when a new piece is dropped in and this one gets kicked out (swap)
    public void ReturnToStart()
    {
        currentSlot = null;
        transform.SetParent(originalParent, true);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        canvasGroup.blocksRaycasts = true;
    }

    // Called by BookSortingManager when the player closes the puzzle early (the "punishment" reset)
    public void ResetToStart()
    {
        ReturnToStart();
    }
}