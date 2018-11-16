using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    NORTH,
    SOUTH,
    EAST,
    WEST
};

public class pacman : MonoBehaviour
{
    private Directions currentDirection;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        currentDirection = Directions.WEST;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == States.CHASE)
        {
            switch(currentDirection)
            {
                case Directions.WEST:
                    rb.MovePosition(rb.position + Vector2.left * 0.075f);
                    break;
            }
        }
    }


}
