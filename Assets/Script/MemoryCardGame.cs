using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Put this on an empty "CardGrid" object inside Task4Screen.
// Every time Task4Screen opens, it builds a fresh shuffled board of cards.
[RequireComponent(typeof(GridLayoutGroup))]
public class MemoryCardGame : MonoBehaviour
{
    [Header("Board")]
    [Tooltip("Must be even. 20 cards = 10 pairs.")]
    [SerializeField] private int totalCards = 20;
    [SerializeField] private int columns = 5;
    [SerializeField] private Vector2 cellSize = new Vector2(100f, 130f);
    [SerializeField] private Vector2 spacing = new Vector2(15f, 15f);

    [Header("Card Sprites (optional)")]
    [Tooltip("One sprite per pair, in order (need at least totalCards / 2). Leave empty to use letters A, B, C...")]
    [SerializeField] private Sprite[] faceSprites;
    [Tooltip("Optional picture for the face-down side of every card. Leave empty for a plain coloured card.")]
    [SerializeField] private Sprite backSprite;

    [Header("Colours (used when no sprites are assigned, and for the matched tint)")]
    [SerializeField] private Color backColor = new Color(0.2f, 0.35f, 0.6f);
    [SerializeField] private Color faceColor = new Color(0.95f, 0.95f, 0.95f);
    [SerializeField] private Color matchedColor = new Color(0.65f, 0.95f, 0.65f);

    [Header("Timing")]
    [SerializeField] private float mismatchDelay = 0.8f;
    [SerializeField] private float winDelay = 0.6f;

    [Header("Events")]
    [Tooltip("Fires once every pair has been matched (after a short delay).")]
    public UnityEvent onAllPairsMatched;

    private class Card
    {
        public int id;
        public bool revealed;
        public bool matched;
        public Sprite faceSprite;
        public Image background;
        public TextMeshProUGUI label;
    }

    private readonly List<Card> cards = new List<Card>();
    private Card firstPick;
    private bool isBusy;
    private int matchedPairs;
    private int totalPairs;

    private void OnEnable()
    {
        BuildBoard();
    }

    private void BuildBoard()
    {
        StopAllCoroutines();

        // clear any old cards from the last time the screen was open
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        cards.Clear();
        firstPick = null;
        isBusy = false;
        matchedPairs = 0;

        // grid layout
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        grid.cellSize = cellSize;
        grid.spacing = spacing;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.childAlignment = TextAnchor.MiddleCenter;

        // two of every id, then shuffle
        totalPairs = Mathf.Max(1, totalCards / 2);
        List<int> ids = new List<int>();
        for (int p = 0; p < totalPairs; p++)
        {
            ids.Add(p);
            ids.Add(p);
        }
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = ids[i];
            ids[i] = ids[j];
            ids[j] = temp;
        }

        foreach (int id in ids)
        {
            CreateCard(id);
        }
    }

    private void CreateCard(int id)
    {
        GameObject go = new GameObject("Card", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(transform, false);

        Image bg = go.GetComponent<Image>();
        bg.preserveAspect = true;

        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = bg;
        btn.transition = Selectable.Transition.None;
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        btn.navigation = nav;

        // text (letter, or "?" while face down) - only used when there is no sprite
        GameObject labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelGO.transform.SetParent(go.transform, false);
        Stretch(labelGO.GetComponent<RectTransform>(), 0f);
        TextMeshProUGUI label = labelGO.GetComponent<TextMeshProUGUI>();
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 56;
        label.fontStyle = FontStyles.Bold;
        label.raycastTarget = false;

        Sprite face = (faceSprites != null && id < faceSprites.Length) ? faceSprites[id] : null;

        Card card = new Card
        {
            id = id,
            faceSprite = face,
            background = bg,
            label = label
        };
        Hide(card);
        cards.Add(card);

        btn.onClick.AddListener(() => OnCardClicked(card));
    }

    private void OnCardClicked(Card card)
    {
        if (isBusy || card.revealed || card.matched) return;

        Reveal(card);

        if (firstPick == null)
        {
            firstPick = card;
            return;
        }

        Card first = firstPick;
        firstPick = null;
        StartCoroutine(CheckPair(first, card));
    }

    private IEnumerator CheckPair(Card a, Card b)
    {
        isBusy = true;

        if (a.id == b.id)
        {
            yield return new WaitForSeconds(0.3f);

            a.matched = true;
            b.matched = true;
            a.background.color = matchedColor;
            b.background.color = matchedColor;
            matchedPairs++;

            if (matchedPairs >= totalPairs)
            {
                yield return new WaitForSeconds(winDelay);
                onAllPairsMatched.Invoke();
                yield break; // stay "busy" so no more clicks while the screen closes
            }
        }
        else
        {
            yield return new WaitForSeconds(mismatchDelay);
            Hide(a);
            Hide(b);
        }

        isBusy = false;
    }

    private void Reveal(Card card)
    {
        card.revealed = true;

        if (card.faceSprite != null)
        {
            card.background.sprite = card.faceSprite;
            card.background.color = Color.white;
            card.label.text = "";
        }
        else
        {
            card.background.sprite = null;
            card.background.color = faceColor;
            card.label.text = card.id < 26 ? ((char)('A' + card.id)).ToString() : (card.id + 1).ToString();
            card.label.color = new Color(0.15f, 0.15f, 0.15f);
        }
    }

    private void Hide(Card card)
    {
        card.revealed = false;

        if (backSprite != null)
        {
            card.background.sprite = backSprite;
            card.background.color = Color.white;
            card.label.text = "";
        }
        else
        {
            card.background.sprite = null;
            card.background.color = backColor;
            card.label.text = "?";
            card.label.color = Color.white;
        }
    }

    private static void Stretch(RectTransform rt, float padding)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(padding, padding);
        rt.offsetMax = new Vector2(-padding, -padding);
    }
}