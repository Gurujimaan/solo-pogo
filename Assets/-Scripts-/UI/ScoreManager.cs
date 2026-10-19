using TMPro;
using UnityEngine;

public class ScoreManager: MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI scoreText;      // Reference to the UI Text component for displaying score
    public TextMeshProUGUI highScoreText;  // Reference to the UI Text component for displaying high score
    public GameObject highScoreStar;

    [Header("Score Settings")]
    public int score = 0;             // Current score
    public int highScore = 0;         // Highest score achieved

    private const string HighScoreKey = "HighScore";

    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString("F0");
        highScoreStar.SetActive(false);
    }


    public void UpdateScore(int newScore)
    {
        if (newScore < score) return; // Prevent score from decreasing

        score = newScore;
        scoreText.text = score.ToString("F0");

        if (score > highScore)
        {
            highScoreStar.SetActive(true);
            highScore = score;
            highScoreText.text = highScore.ToString("F0");
            SaveHighScore();
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save(); 
    }

    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (highScoreText != null) highScoreText.text = highScore.ToString("F0");
    }

    [ContextMenu("Reset Saved High Score")]
    public void DeleteSavedHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        highScore = 0;
        if (highScoreText != null) highScoreText.text = highScore.ToString("F0");
    }
}
