using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    private Directions lastInput;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObject = new GameObject("InputManager");
                instance = managerObject.AddComponent<InputManager>();
            }

            return instance;
        }
    }
    private void Start()
    {
        lastInput = Directions.WEST;
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") < 0)
        {
            lastInput = Directions.WEST;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            lastInput = Directions.EAST;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            lastInput = Directions.SOUTH;
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            lastInput = Directions.NORTH;
        }

        if (( GameManager.Instance.gameState == States.GAMEOVER ||
              GameManager.Instance.gameState == States.WIN ) && 
              Input.GetButton("Submit"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    public Directions GetLastInput()
    {
        return lastInput;
    }
}
