using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FaintSequenceManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The UI Image that covers the screen")]
    public Image blackoutScreen;

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip fallSound;

    [Header("Dialogue Trigger")]
    public string npcName = "JOHNNY";
    [TextArea(2, 5)]
    public string[] blackoutDialogueLines;

    [Header("Teleportation")]
    public Transform playerTransform;
    public Transform merchantRoomSpawnPoint;

    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();

        // Ensure the screen starts completely clear
        if (blackoutScreen != null)
        {
            Color c = blackoutScreen.color;
            c.a = 0f;
            blackoutScreen.color = c;
            blackoutScreen.gameObject.SetActive(false);
        }
    }

    public void StartFaintingSequence()
    {
        StartCoroutine(FaintRoutine());
    }

    private IEnumerator FaintRoutine()
    {
        blackoutScreen.gameObject.SetActive(true);

        // --- BLINK 1 (Heavy eyelids) ---
        yield return FadeToAlpha(0.6f, 0.4f); // Fade to 60% black
        yield return FadeToAlpha(0.0f, 0.3f); // Open eyes

        // --- BLINK 2 (Almost passed out) ---
        yield return FadeToAlpha(0.85f, 0.4f); // Fade to 85% black
        yield return FadeToAlpha(0.0f, 0.3f);  // Open eyes weak

        // --- FINAL FADE (Pitch Black) ---
        yield return FadeToAlpha(1.0f, 0.8f); // Fade to 100% black

        // Play the heavy fall sound effect (if you assigned one)
        if (audioSource != null && fallSound != null)
        {
            audioSource.PlayOneShot(fallSound);
            yield return new WaitForSeconds(1.0f);
        }

        // --- TRIGGER DIALOGUE ---
        // Tell DialogueManager: "When this text finishes, run my Delay routine!"
        dialogueManager.onDialogueFinished += OnBlackoutDialogueFinished;
        dialogueManager.StartDialogue(npcName, blackoutDialogueLines);
    }

    // Helper function to handle fading smoothly
    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        Color color = blackoutScreen.color;
        float startAlpha = color.a;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            blackoutScreen.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        blackoutScreen.color = color;
    }

    private void OnBlackoutDialogueFinished()
    {
        // Unsubscribe immediately so it only happens once
        dialogueManager.onDialogueFinished -= OnBlackoutDialogueFinished;

        // Start the final delay before teleporting
        StartCoroutine(WakeUpRoutine());
    }

    private IEnumerator WakeUpRoutine()
    {
        // 1. Wait 1.5 seconds in the dark AFTER dialogue closes
        yield return new WaitForSeconds(1.5f);

        // 2. Teleport the player
        playerTransform.position = merchantRoomSpawnPoint.position;

        // 3. Wake up! (Instantly clear the screen)
        Color c = blackoutScreen.color;
        c.a = 0f;
        blackoutScreen.color = c;
        blackoutScreen.gameObject.SetActive(false);
    }
}