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

    private int score;
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


    private void Start()
    {
        score = 0;
        gameState = States.CHASE;
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
    }

    private void Update()
    {
        
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
