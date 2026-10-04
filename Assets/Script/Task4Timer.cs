using UnityEngine;
using TMPro;
using UnityEngine.Events;
using StarterAssets; // NEW: for FirstPersonController and StarterAssetsInputs

public class Task4Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startingSeconds = 60f;
    [SerializeField] private bool startWhenEnabled = true;

    public UnityEvent onTimerFinished;

    private float secondsRemaining;
    private bool isRunning;

    // NEW: same player references the ShopManager uses
    private FirstPersonController playerController;
    private StarterAssetsInputs starterInputs;
    private PlayerInteract playerInteract; // stops F from talking to the NPC while the screen is open

    private void OnEnable()
    {
        // NEW
        playerController = FindAnyObjectByType<FirstPersonController>();
        starterInputs = FindAnyObjectByType<StarterAssetsInputs>();
        playerInteract = FindAnyObjectByType<PlayerInteract>();
        ApplyOpenState();

        ResetTimer();

        if (startWhenEnabled)
        {
            BeginTimer();
        }
    }

    // NEW: re-applies every frame while the screen is open, so if the dialogue
    // closing (or anything else) re-locks the mouse or unfreezes the player
    // after we opened, we simply undo it again.
    private void LateUpdate()
    {
        ApplyOpenState();
    }

    // NEW: runs when the screen closes (Cancel button / SetActive(false))
    private void OnDisable()
    {
        if (playerController != null) playerController.enabled = true;
        if (playerInteract != null) playerInteract.enabled = true;

        if (starterInputs != null)
        {
            starterInputs.cursorLocked = true;
            starterInputs.cursorInputForLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // NEW: exactly what ShopManager.OpenShop() does
    private void ApplyOpenState()
    {
        if (playerController != null && playerController.enabled) playerController.enabled = false;
        if (playerInteract != null && playerInteract.enabled) playerInteract.enabled = false;

        if (starterInputs != null)
        {
            starterInputs.cursorLocked = false;
            starterInputs.cursorInputForLook = false;
        }

        if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
        if (!Cursor.visible) Cursor.visible = true;
    }

    private void Update()
    {
        if (!isRunning) return;

        secondsRemaining -= Time.deltaTime;

        if (secondsRemaining <= 0f)
        {
            secondsRemaining = 0f;
            isRunning = false;
            UpdateTimerText();
            onTimerFinished.Invoke();
            return;
        }

        UpdateTimerText();
    }

    public void BeginTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        secondsRemaining = startingSeconds;
        UpdateTimerText();
    }

    public void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(secondsRemaining);
        timerText.text = $"Timer: {seconds}s";
    }
}