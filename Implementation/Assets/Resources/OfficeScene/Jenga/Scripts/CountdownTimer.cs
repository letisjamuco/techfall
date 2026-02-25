using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float startTime = 80f; // 2 minutes example

    // Works with BOTH UI and World Text
    [SerializeField] private TMP_Text timerText;

    [SerializeField] UnityEvent endOfTimeEvent;

    private float currentTime;
    private bool isRunning;

    private void Start()
    {
        ResetTimer();
    }

    public void StartTimer()
    {
        if (isRunning) return;

        isRunning = true;
        StartCoroutine(RunTimer());
    }

    public void ResetTimer()
    {
        StopAllCoroutines();
        isRunning = false;
        currentTime = startTime;
        UpdateUI();
    }

    private IEnumerator RunTimer()
    {
        while (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }

        currentTime = 0f;
        UpdateUI();
        isRunning = false;

        TimerFinished();
    }

    private void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void TimerFinished()
    {
       endOfTimeEvent.Invoke(); 
    }
}
