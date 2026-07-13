using System.Collections;
using UnityEngine;
using TMPro;
using StarterAssets; // <--- NEW: Tells the script where to find your FirstPersonController

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.04f;

    [Header("Scripts")]
    public MonoBehaviour interactScript;
    public FirstPersonController playerController; // <--- NEW: Slot for your movement script

    [HideInInspector] public bool isTyping = false;
    private string[] sentences;
    private int currentLineIndex = 0;
    private string currentSentenceText = "";
    private Coroutine typingCoroutine;
    private bool canProceed = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (dialoguePanel.activeInHierarchy && canProceed && Input.GetKeyDown(KeyCode.F))
        {
            if (isTyping)
            {
                CompleteSentence();
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(string npcName, string[] dialogueLines)
    {
        dialoguePanel.SetActive(true);
        canProceed = false;

        nameText.text = npcName;
        sentences = dialogueLines;
        currentLineIndex = 0;

        // --- NEW: Freeze the player! ---
        if (interactScript != null) interactScript.enabled = false;
        if (playerController != null) playerController.enabled = false;

        DisplayNextSentence();
        StartCoroutine(UnlockInputDelay());
    }

    private IEnumerator UnlockInputDelay()
    {
        yield return new WaitForSeconds(0.2f);
        canProceed = true;
    }

    public void DisplayNextSentence()
    {
        if (currentLineIndex < sentences.Length)
        {
            currentSentenceText = sentences[currentLineIndex];

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeSentence(currentSentenceText));

            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    public void CompleteSentence()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = currentSentenceText;
        isTyping = false;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        StartCoroutine(RestoreInteraction());
    }

    private IEnumerator RestoreInteraction()
    {
        yield return null;

        // --- NEW: Unfreeze the player! ---
        if (interactScript != null) interactScript.enabled = true;
        if (playerController != null) playerController.enabled = true;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}