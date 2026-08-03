using TMPro;
using UnityEngine;

public class ScoreManager: MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI scoreText;      // Reference to the UI Text component for displaying score
    public TextMeshProUGUI highScoreText;  // Reference to the UI Text component for displaying high score
    public GameObject highScoreStar;

    [Header("Score Settings")]
    public float score = 0.0f;             // Current score
    public float highScore = 0.0f;         // Highest score achieved


    public void ResetScore()
    {
        score = 0.0f;
        scoreText.text = score.ToString("F0");
        highScoreStar.SetActive(false);
    }


    public void UpdateScore(float newScore)
    {
        if (newScore < score) return; // Prevent score from decreasing

        score = newScore;
        scoreText.text = score.ToString("F0");

        if (score > highScore)
        {
            highScoreStar.SetActive(true);
            highScore = score;
            highScoreText.text = highScore.ToString("F0");
        }
    }
}
