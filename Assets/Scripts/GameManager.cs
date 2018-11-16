using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static GameManager Instance;

    public States gameState;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        gameState = States.CHASE;
    }

    private void Update()
    {
        
    }

}
