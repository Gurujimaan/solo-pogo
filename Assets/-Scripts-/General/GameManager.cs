using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public GameObject player;
    public RisingAcid risingAcid;
    public ScoreManager scoreManager;
    public LevelGenerator levelGenerator;

    [Header("Game Settings")]
    public float acidHeightReq = 10;
    public float playerAcidGap = 14;

    private float playerHeight;
    private bool gameStarted = false;


    private void Start()
    {
        StartGame();
    }


    void Update()                //Check when to start rising the acid
    {
        if(gameStarted)
        {
            playerHeight = player.transform.position.y;
            scoreManager.UpdateScore((int)playerHeight);

            if(playerHeight > acidHeightReq)
            {
                risingAcid.isRising = true;
            }
        }
    }


    public void StartGame()         //Reset Score UI
    {
        gameStarted = true;
        risingAcid.ResetAcid(); 
        scoreManager.ResetScore();
        scoreManager.LoadHighScore();
        playerHeight = 0;
    }


    public void PlayerDeath()
    {
        gameStarted = false;
        risingAcid.isRising = false;
        Destroy(player);
    }
}
