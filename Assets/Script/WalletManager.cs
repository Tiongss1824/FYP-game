using UnityEngine;
using TMPro;
using System.Collections;

public class WalletManager : MonoBehaviour
{
    // The Singleton - makes this script globally accessible
    public static WalletManager Instance;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI cashTextUI;

    [Header("Popup UI Settings")]
    [SerializeField] private TextMeshProUGUI cashPopupText; // The floating +50 text
    [SerializeField] private float popupDuration = 1.5f;    // How long it floats before vanishing
    [SerializeField] private float floatHeight = 50f;       // How high it floats up

    private int currentCash = 0;

    private void Awake()
    {
        // Set up the Singleton when the game boots
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevents accidental duplicates
        }
    }

    private void Start()
    {
        UpdateUI();

        // Hide the popup text when the game starts
        if (cashPopupText != null)
        {
            cashPopupText.gameObject.SetActive(false);
        }
    }

    public void AddCash(int amount)
    {
        currentCash += amount;
        UpdateUI();

        // Show the cool floating popup!
        if (cashPopupText != null)
        {
            StopAllCoroutines(); // Reset just in case you get money twice very fast
            StartCoroutine(AnimatePopup(amount));
        }
    }

    public bool TryBuyMedicine(int cost)
    {
        if (currentCash >= cost)
        {
            currentCash -= cost;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough cash!");
            return false;
        }
    }

    private void UpdateUI()
    {
        if (cashTextUI != null)
        {
            cashTextUI.text = "Cash: $ " + currentCash.ToString();
        }
    }

    private IEnumerator AnimatePopup(int amount)
    {
        // 1. Setup the text and show it
        cashPopupText.text = "+ $" + amount.ToString();
        cashPopupText.gameObject.SetActive(true);

        // 2. Setup the colors for fading
        Color originalColor = cashPopupText.color;
        Color fadeColor = originalColor;
        fadeColor.a = 1f; // Fully solid
        cashPopupText.color = fadeColor;

        // 3. Setup the positions for floating
        Vector3 startPos = cashPopupText.rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, floatHeight, 0);

        float elapsed = 0f;

        // 4. Do the fade and float animation
        while (elapsed < popupDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popupDuration;

            // Fade out math
            fadeColor.a = Mathf.Lerp(1f, 0f, t);
            cashPopupText.color = fadeColor;

            // Float up math
            cashPopupText.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // 5. Hide it and snap it back to the original spot for next time
        cashPopupText.gameObject.SetActive(false);
        cashPopupText.rectTransform.anchoredPosition = startPos;
        cashPopupText.color = originalColor; // Reset color
    }
}