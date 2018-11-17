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
    private Text scoreText;

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
        gameState = States.CHASE;
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();

        ghosts = new List<Sprite>();
        ghosts.Add(FindObjectOfType<Blinky>());
        ghosts.Add(FindObjectOfType<Pinky>());
    }

    private void Update()
    {
        if (ghosts[1].waiting && Time.time >= 2)
        {
            ghosts[1].waiting = false;
            ghosts[1].spawning = true;
        }
    }

    public void IncrementScore(int inc)
    {
        score += inc;
        scoreText.text = "Score : " + score;
    }

    public void SetState(States newState)
    {
        gameState = newState;
    }

}
