using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TMP_Text scoreText;
    private int score = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
{
    score += points;
    UpdateScoreText();
}
    
    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

   
    public int GetScore()
    {
        return score;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogWarning("GameManager: scoreText is not assigned in the Inspector.");
        }
    }
}