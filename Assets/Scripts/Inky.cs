using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    private Pacman player;
    private Blinky blinky;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        blinky = FindObjectOfType<Blinky>();
        scatterTarget = new Vector2(13.5f, -2.5f);

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
            // Inky's target uses both the player and Blinky
            Vector2 tempTargetPosition = (Vector2)player.transform.position + (GetVectorFromDirection(player.GetCurrentDirection()) * 1.0f);
            Vector2 blinkyPosition = blinky.transform.position;
            Vector2 direction = (tempTargetPosition - blinkyPosition).normalized;
            float mag = (tempTargetPosition - blinkyPosition).magnitude;
            targetPosition = blinkyPosition + direction * mag * 2.0f;
        }

        return targetPosition;
    }
}
