using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Tooltip("Panel shown on game over. Assign in the Inspector.")]
    public GameObject gameOverPanel;

    [Tooltip("Text component on the Game Over panel that shows the final score.")]
    public TMP_Text gameOverScoreText;

    [Tooltip("Text component on the Game Over panel that shows the final time.")]
    public TMP_Text gameOverTimeText; // New reference for time text

    public bool dontDestroyOnLoad = false;
    [Tooltip("Name of the main menu scene to load when MainMenu() is called.")]
    public string mainMenuSceneName = "Menu";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            Debug.LogWarning("Another GameOverManager instance already exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameOverManager: gameOverPanel is not assigned in the Inspector.");
        }
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // If a GameManager exists and a score text is assigned, show the final score.
            if (gameOverScoreText != null)
            {
                if (GameManager.instance != null)
                {
                    gameOverScoreText.text = "Score: " + GameManager.instance.GetScore().ToString();
                }
                else
                {
                    gameOverScoreText.text = "Score: -";
                }
            }

            // If a Timer exists and a time text is assigned, show the final time.
            if (gameOverTimeText != null)
            {
                if (Timer.instance != null)
                {
                    float time = Timer.instance.GetTime();
                    gameOverTimeText.text = "Time: " + time.ToString("F2") + "s"; // Show time with 2 decimal places
                }
                else
                {
                    gameOverTimeText.text = "Time: -";
                }
            }
        }
        else
        {
            Debug.LogWarning("GameOverManager.GameOver called but gameOverPanel is null.");
        }

        Time.timeScale = 0f; // Pause the game
    }

    public void Restart()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetScore();
        }

        if (Timer.instance != null)
        {
            Timer.instance.ResetTimer();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetScore();
        }

        if (Timer.instance != null)
        {
            Timer.instance.ResetTimer();
        }

        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("GameOverManager: mainMenuSceneName is empty — set it in the Inspector to load your main menu scene.");
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}