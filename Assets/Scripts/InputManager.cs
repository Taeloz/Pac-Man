using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public Directions getLastInput()
    {
        return lastInput;
    }
}
