using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    private Pacman player;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        scatterTarget = new Vector2(13.5f, 16.5f);

        speed = 2.5f;
        normalSpeed = speed;

        waiting = false;
        spawning = false;
    }

    protected override Vector2 GetTargetPosition()
    {
        Vector2 targetPosition;

        if (returningToSpawn)
        {
            targetPosition = new Vector2(6.5f, 8.5f);
        }
        else if (GameManager.Instance.gameState == States.SCATTER)
        {
            // Blinky's scatter target is player after a certain point
            if (speed >= 3.0f)
            {
                targetPosition = player.transform.position;
            }
            else
            {
                targetPosition = scatterTarget;
            }
        }
        else
        {
            // Blinky's target is the player's position
            targetPosition = player.transform.position;
        }

        return targetPosition;
    }
}
