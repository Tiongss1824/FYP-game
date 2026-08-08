using UnityEngine;
using TMPro;

// One step in your task sequence, editable in the Inspector.
// e.g. "Visit Friend", or "Collect Cabbages" with hasProgress=true, targetAmount=6
[System.Serializable]
public class TaskStep
{
    public string label;
    [Tooltip("Tick this if the task needs a counter, like 'Collect Cabbages 0/6'")]
    public bool hasProgress;
    [Tooltip("Only used if Has Progress is ticked")]
    public int targetAmount = 1;
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("UI Reference")]
    [Tooltip("The TextMeshProUGUI in the upper-left corner of your Canvas")]
    public TextMeshProUGUI taskText;

    [Header("Task Sequence")]
    [Tooltip("Set up your full chain of instructions here in order, e.g. Visit Friend -> Collect Cabbages -> Buy Medicine")]
    public TaskStep[] taskSequence;

    [Tooltip("If ticked, taskSequence[0] shows automatically as soon as the game starts")]
    public bool autoStartFirstTask = true;

    [Header("Task 0: The Friend")]
    [Tooltip("Drag the friend's GameObject (the one with NpcTalk on it)")]
    public NpcTalk friendTalk;

    [Header("Task 1: The Old Man / Cabbage Quest")]
    [Tooltip("Drag the old man's GameObject (the one with NpcTalk on it)")]
    public NpcTalk oldManTalk;

    [Tooltip("Drag the object with CountVege on it (the basket / trigger zone)")]
    public CountVege vegetableCounter;

    [Tooltip("The index in Task Sequence above for 'Collect Cabbages'. Must match the list order.")]
    public int cabbageTaskIndex = 1;

    [Header("Task 2: Arrange Book NPC")]
    [Tooltip("Drag the task 2 NPC's GameObject (the one with NpcTalk on it)")]
    public NpcTalk task2Talk;

    // --- Internal state ---
    private int currentTaskIndex = -1;
    private string taskLabel = "";
    private int currentAmount = 0;
    private int targetAmount = 0;
    private bool showProgress = false;

    private bool hasClearedVisitFriend = false;
    private bool hasShownCabbageTask = false;
    private bool hasAdvancedPastCabbageTask = false;
    private bool hasShownTask2 = false;
    private bool hasAdvancedPastTask2 = false;

    private void Awake()
    {
        // Simple singleton so any script can call TaskManager.Instance from anywhere
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (taskText != null) taskText.gameObject.SetActive(false);

        if (autoStartFirstTask && taskSequence.Length > 0)
        {
            GoToTask(0);
        }
    }

    private void Update()
    {
        // --- Task 0: Visit Friend ---
        // The moment the player talks to the friend, clear the task (go blank)
        // instead of immediately showing the next one.
        if (!hasClearedVisitFriend && friendTalk != null && friendTalk.HasBeenAssigned)
        {
            hasClearedVisitFriend = true;
            ClearTask();
        }

        // --- Task 1: The old man / cabbage quest ---
        // Only appears once the player has visited the friend AND talks to the old man.
        if (hasClearedVisitFriend && !hasShownCabbageTask && oldManTalk != null && oldManTalk.HasBeenAssigned)
        {
            hasShownCabbageTask = true;
            GoToTask(cabbageTaskIndex);
        }

        // While the cabbage task is active, keep the on-screen counter synced
        if (hasShownCabbageTask && !hasAdvancedPastCabbageTask && vegetableCounter != null)
        {
            SetProgress(vegetableCounter.CurrentVeggies);
        }

        // Once the old man's quest is fully marked complete, clear the task (go blank)
        // instead of immediately showing "Arrange book".
        if (hasShownCabbageTask && !hasAdvancedPastCabbageTask && oldManTalk != null && oldManTalk.isTaskCompleted)
        {
            hasAdvancedPastCabbageTask = true;
            ClearTask();
        }

        // --- Task 2: Arrange book ---
        // Only appears once the player talks to the task 2 NPC.
        if (hasAdvancedPastCabbageTask && !hasShownTask2 && task2Talk != null && task2Talk.HasBeenAssigned)
        {
            hasShownTask2 = true;
            AdvanceToNextTask(); // moves from the cabbage task's index to the next one, e.g. "Arrange book"
        }

        // Once the book-sorting puzzle is fully complete, clear the task (go blank)
        if (hasShownTask2 && !hasAdvancedPastTask2 && task2Talk != null && task2Talk.isTaskCompleted)
        {
            hasAdvancedPastTask2 = true;
            ClearTask();
        }

        // --- Add Task 3+ watching here later, following the same clear-then-show pattern ---
    }

    // --- Sequence control ---

    // Jump directly to a specific step in taskSequence (0 = first task, 1 = second, etc.)
    public void GoToTask(int index)
    {
        if (index < 0 || index >= taskSequence.Length)
        {
            ClearTask();
            return;
        }

        currentTaskIndex = index;
        TaskStep step = taskSequence[index];

        if (step.hasProgress)
        {
            SetTaskWithProgress(step.label, step.targetAmount);
        }
        else
        {
            SetTask(step.label);
        }
    }

    // Call this when the current step is done, to move on to the next one in the list.
    public void AdvanceToNextTask()
    {
        GoToTask(currentTaskIndex + 1);
    }

    // --- Manual task control (still usable directly if you don't want to use the sequence) ---

    public void SetTask(string label)
    {
        taskLabel = label;
        showProgress = false;
        RefreshUI();
    }

    public void SetTaskWithProgress(string label, int target, int startingAmount = 0)
    {
        taskLabel = label;
        targetAmount = target;
        currentAmount = startingAmount;
        showProgress = true;
        RefreshUI();
    }

    // Call this every time the player picks up a vegetable (or whatever you're counting)
    public void AddProgress(int amount = 1)
    {
        if (!showProgress) return; // Safety: ignore if no counting task is active

        currentAmount += amount;
        if (currentAmount > targetAmount) currentAmount = targetAmount;

        RefreshUI();

        if (currentAmount >= targetAmount)
        {
            OnTaskProgressComplete();
        }
    }

    // Sets progress to an exact value instead of incrementing —
    // used by the Task 1 watcher above to sync against CountVege's own counter.
    public void SetProgress(int amount)
    {
        if (!showProgress) return;

        currentAmount = Mathf.Clamp(amount, 0, targetAmount);
        RefreshUI();

        if (currentAmount >= targetAmount)
        {
            OnTaskProgressComplete();
        }
    }

    // Optional: hook for when the counter hits its target (e.g. play a sound, show a prompt)
    private void OnTaskProgressComplete()
    {
        Debug.Log("Task progress complete: " + taskLabel);
    }

    public void ClearTask()
    {
        taskLabel = "";
        showProgress = false;
        if (taskText != null) taskText.gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        if (taskText == null) return;

        taskText.gameObject.SetActive(true);

        if (showProgress)
        {
            taskText.text = taskLabel + "  " + currentAmount + "/" + targetAmount;
        }
        else
        {
            taskText.text = taskLabel;
        }
    }
}