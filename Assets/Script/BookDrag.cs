using UnityEngine;
using UnityEngine.EventSystems;

// Attach this to each draggable book UI Image.
// The book's own Image color IS its "color tag" — BookSlot compares against it directly.
[RequireComponent(typeof(CanvasGroup))]
public class BookDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private Vector2 originalAnchoredPosition;
    private Transform originalParent;

    [HideInInspector] public bool isPlaced = false; // true once correctly slotted

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();

        // Remember this as "home" permanently, so resets always return here
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return; // don't let already-correct books be dragged again

        // Move to the top of the canvas hierarchy while dragging so it renders above everything else
        transform.SetParent(rootCanvas.transform, true);
        canvasGroup.blocksRaycasts = false; // lets BookSlot detect the drop underneath the book
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        canvasGroup.blocksRaycasts = true;

        // If nothing accepted the drop (BookSlot didn't call SnapInto), return to start
        if (transform.parent == rootCanvas.transform)
        {
            ReturnToStart();
        }
    }

    // Called by BookSlot when the color matches and it accepts this book
    public void SnapInto(RectTransform slot)
    {
        isPlaced = true;
        transform.SetParent(slot, true);
        rectTransform.anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = false; // placed books no longer need to block/receive drags
    }

    private void ReturnToStart()
    {
        transform.SetParent(originalParent, true);
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }

    // Called by BookSortingManager when the player closes the puzzle early (the "punishment" reset)
    public void ResetToStart()
    {
        isPlaced = false;
        transform.SetParent(originalParent, true);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        canvasGroup.blocksRaycasts = true;
    }
}