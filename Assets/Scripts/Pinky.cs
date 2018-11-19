using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    private Pacman player;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        scatterTarget = new Vector2(-0.5f, 16.5f);

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
            // Pinky's target is ahead of the player
            targetPosition = (Vector2)player.transform.position + (GetVectorFromDirection(player.GetCurrentDirection()) * 2.0f);
        }

        return targetPosition;
    }
}