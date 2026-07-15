using UnityEngine;
using TMPro;

public class CountVege : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private int requiredVeggies = 5;
    private int currentVeggies = 0;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI questTextUI;

    [Tooltip("Text to display on the screen when the basket is full")]
    [SerializeField] private string taskCompleteText = "Task Complete!"; // <--- NEW

    [Header("The Old Man")]
    // --- CHANGED: Now looks for your upgraded universal script! ---
    [SerializeField] private NpcTalk oldManScript;

    [Header("The Old Man Cutscene")]
    [SerializeField] private CinematicTrigger oldManCutscene;

    private bool questCompleted = false;

    private void Start()
    {
        UpdateQuestUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (questCompleted) return;

        if (other.CompareTag("Vegetables"))
        {
            currentVeggies++;
            UpdateQuestUI();

            if (currentVeggies >= requiredVeggies)
            {
                CompleteQuest();
            }
        }
    }

    private void UpdateQuestUI()
    {
        if (questTextUI != null)
        {
            questTextUI.text = $"{currentVeggies} / {requiredVeggies}";
        }
    }

    private void CompleteQuest()
    {
        questCompleted = true;

        if (questTextUI != null)
        {
            // --- CHANGED: Now uses your custom Inspector text! ---
            questTextUI.text = taskCompleteText;
        }

        // --- CHANGED: Flips the boolean in NPCtalk to swap to the After Task dialogue ---
        if (oldManScript != null)
        {
            oldManScript.CompleteTask();
        }

        // Start the cinematic camera!
        if (oldManCutscene != null)
        {
            oldManCutscene.StartCutsceneManually();
        }
    }
}