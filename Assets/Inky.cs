﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Sprite
{
    public LayerMask raycastLayerMask;

    public SpriteRenderer currentEyes;
    public UnityEngine.Sprite[] allEyes;

    private Pacman player;
    private Blinky blinky;

    private Directions lastDirection;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        blinky = FindObjectOfType<Blinky>();
        speed = 0.05f;
        lastMotion = Directions.WEST;
        waiting = true;
    }

    public override Directions GetMovementDirection()
    {
        if (!waitingToChangeDirection)
        {
            // Inky's target uses both the player and Blinky
            Vector2 tempTargetPosition = (Vector2)player.transform.position + (GetVectorFromDirection(player.GetCurrentDirection()) * 1.0f);
            Vector2 blinkyPosition = blinky.transform.position;
            Vector2 direction = (tempTargetPosition - blinkyPosition).normalized;
            float mag = (tempTargetPosition - blinkyPosition).magnitude;
            Vector2 targetPosition = blinkyPosition + direction * mag * 2.0f;

            Vector2 movementDir = GetVectorFromDirection(currentDirection);

            // Find the next upcoming potential intersection
            Vector2 nextTile;
            nextTile.x = Mathf.Round((rb.position.x) * 2.0f) / 2.0f + movementDir.x * 0.6f;
            nextTile.y = Mathf.Round((rb.position.y) * 2.0f) / 2.0f + movementDir.y * 0.6f;

            // Check all directions from that intersection
            RaycastHit2D raycastNorth = Physics2D.CircleCast(nextTile, 0.4f, Vector2.up, 1.0f, raycastLayerMask);
            RaycastHit2D raycastSouth = Physics2D.CircleCast(nextTile, 0.4f, Vector2.down, 1.0f, raycastLayerMask);
            RaycastHit2D raycastEast = Physics2D.CircleCast(nextTile, 0.4f, Vector2.right, 1.0f, raycastLayerMask);
            RaycastHit2D raycastWest = Physics2D.CircleCast(nextTile, 0.4f, Vector2.left, 1.0f, raycastLayerMask);

            // Get a list of all the possible directions from that intersection
            List<Vector2> possibleDirections = new List<Vector2>();

            if (raycastWest.collider == null && currentDirection != Directions.EAST && lastMotion != Directions.EAST)
            {
                possibleDirections.Add(nextTile + Vector2.left * 1.0f);
            }
            if (raycastNorth.collider == null && currentDirection != Directions.SOUTH && lastMotion != Directions.SOUTH)
            {
                possibleDirections.Add(nextTile + Vector2.up * 1.0f);
            }
            if (raycastEast.collider == null && currentDirection != Directions.WEST && lastMotion != Directions.WEST)
            {
                possibleDirections.Add(nextTile + Vector2.right * 1.0f);
            }
            if (raycastSouth.collider == null && currentDirection != Directions.NORTH && lastMotion != Directions.NORTH)
            {
                possibleDirections.Add(nextTile + Vector2.down * 1.0f);
            }

            // If there is at least one possible direction, check for the best one
            Vector2 bestChoice;
            if (possibleDirections.Count > 0)
            {
                waitingToChangeDirection = true;

                bestChoice = possibleDirections[0];

                for (int i = 0; i < possibleDirections.Count; i++)
                {
                    if ((targetPosition - possibleDirections[i]).magnitude < (targetPosition - bestChoice).magnitude)
                    {
                        bestChoice = possibleDirections[i];
                    }
                }
            }
            else
            {
                return lastDirection;
            }

            lastDirection = GetDirectionFromVector((bestChoice - nextTile).normalized);
        }

        return lastDirection;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentDirection)
        {
            case Directions.EAST:
                currentEyes.sprite = allEyes[3];
                break;
            case Directions.WEST:
                currentEyes.sprite = allEyes[2];
                break;
            case Directions.NORTH:
                currentEyes.sprite = allEyes[0];
                break;
            case Directions.SOUTH:
                currentEyes.sprite = allEyes[1];
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Pacman>())
        {
            GameManager.Instance.SetState(States.DEATH);
            collision.collider.GetComponent<Pacman>().Die();
        }
    }
}