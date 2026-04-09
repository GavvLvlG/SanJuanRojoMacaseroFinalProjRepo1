 using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    [Header("UI")]
    public TextMeshProUGUI timerText;  

    [Header("Timer Settings")]
    public bool timerRunning = true; 
    public float elapsedTime = 0f;   

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Another Timer instance already exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay(elapsedTime);
        }
    }

    // Stops the timer
    public void StopTimer()
    {
        timerRunning = false;
    }

    // Starts or resumes the timer
    public void StartTimer()
    {
        timerRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay(elapsedTime);
        timerRunning = true;
    }


    void UpdateTimerDisplay(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerText != null)
        {
            timerText.text = timeString;
        }
    }
}