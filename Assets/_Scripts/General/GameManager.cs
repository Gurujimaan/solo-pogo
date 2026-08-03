using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public GameObject player;
    public RisingAcid risingAcid;
    public ScoreManager scoreManager;

    [Header("Game Settings")]
    public float acidHeightReq = 10;

    private float height;
    private bool gameStarted = false;

    private void Start()
    {
        StartGame();
    }

    void Update()
    {
        if(gameStarted)
        {
            height = player.transform.position.y;
            scoreManager.UpdateScore(height);

            if(height > acidHeightReq)
            {
                risingAcid.isRising = true;
            }
        }
    }


    public void StartGame()
    {
        gameStarted = true;
        risingAcid.isRising = false;
        scoreManager.ResetScore();
        height = 0;
    }


    public void PlayerDeath()
    {
        gameStarted = false;
        risingAcid.isRising = false;
        Destroy(player);
    }
}
