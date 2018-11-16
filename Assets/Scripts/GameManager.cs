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

    private InputManager inputManager;

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
        gameState = States.CHASE;
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        
    }

}
