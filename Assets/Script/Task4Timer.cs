using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Task4Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startingSeconds = 60f;
    [SerializeField] private bool startWhenEnabled = true;

    public UnityEvent onTimerFinished;

    private float secondsRemaining;
    private bool isRunning;

    private void OnEnable()
    {
        ResetTimer();

        if (startWhenEnabled)
        {
            BeginTimer();
        }
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

        UpdateTimerText ();
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