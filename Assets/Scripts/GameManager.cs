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
    private static GameManager instance;

    private States gameState;

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameManager();
            }

            return instance;
        }
    }

    private void Start()
    {
        gameState = States.INTRO;
    }

    private void Update()
    {
        
    }

}
