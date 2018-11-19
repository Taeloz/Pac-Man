using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    private Pacman player;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        scatterTarget = new Vector2(-0.5f, -2.5f);

        speed = 2.5f;
        normalSpeed = speed;

        waiting = true;
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
            targetPosition = scatterTarget;
        }
        else
        {
            // Clyde targets the player if greater than a certain distance, otherwise bottom left corner
            targetPosition = player.transform.position;
            if ((targetPosition - (Vector2)transform.position).magnitude < 4.0f)
            {
                targetPosition = scatterTarget;
            }
        }

        return targetPosition;
    }
}
