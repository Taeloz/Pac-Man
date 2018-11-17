using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum States
{
    INTRO,
    CHASE,
    SCATTER,
    FLEE,
    DEATH,
    GAMEOVER
};

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private List<Sprite> ghosts;

    private int score = 0;
    private int pelletsEaten = 0;
    private int totalPellets = 0;
    private Text scoreText;
    private Text readyText;

    public States gameState;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObject = new GameObject("GameManager");
                instance = managerObject.AddComponent<GameManager>();
            }

            return instance;
        }
    }


    private void Awake()
    {
        score = 0;
        gameState = States.INTRO;
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        readyText = GameObject.FindGameObjectWithTag("ReadyText").GetComponent<Text>();

        ghosts = new List<Sprite>();
        ghosts.Add(FindObjectOfType<Blinky>());
        ghosts.Add(FindObjectOfType<Pinky>());
        ghosts.Add(FindObjectOfType<Inky>());
        ghosts.Add(FindObjectOfType<Clyde>());
    }

    private void Update()
    {
        // Take care of the opening
        if (gameState == States.INTRO && Time.time >= 2)
        {
            gameState = States.CHASE;
            readyText.enabled = false;
        }

        // Spawn ghosts at appropriate times
        if (gameState == States.CHASE || gameState == States.SCATTER)
        {
            if (ghosts[1].waiting && Time.time >= 4)
            {
                ghosts[1].waiting = false;
                ghosts[1].spawning = true;
            }
            if (ghosts[2].waiting && pelletsEaten >= 40)
            {
                ghosts[2].waiting = false;
                ghosts[2].spawning = true;
            }
            if (ghosts[3].waiting && pelletsEaten >= totalPellets / 3)
            {

                ghosts[3].waiting = false;
                ghosts[3].spawning = true;
            }
        }
        
    }

    public void IncrementScore(int inc)
    {
        score += inc;
        scoreText.text = "Score : " + score;
    }

    public void IncrementPellets()
    {
        pelletsEaten++;
    }

    public void IncrementTotalPellets()
    {
        totalPellets++;
    }

    public void SetState(States newState)
    {
        gameState = newState;
    }

}
