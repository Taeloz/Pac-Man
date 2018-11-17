using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Sprite
{
    private Pacman player;

    private Directions lastDirection;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        speed = 0.05f;
        lastMotion = Directions.WEST;
    }

    public override Directions GetMovementDirection()
    {
        if (!waitingToChangeDirection)
        {

            Vector2 playerPosition = player.transform.position;

            Vector2 movementDir = GetVectorFromDirection(currentDirection);

            // Find the next upcoming potential intersection
            Vector2 nextTile;
            nextTile.x = Mathf.Round((rb.position.x) * 2.0f) / 2.0f + movementDir.x * 0.5f;
            nextTile.y = Mathf.Round((rb.position.y) * 2.0f) / 2.0f + movementDir.y * 0.5f;

            // Check all directions from that intersection
            RaycastHit2D raycastNorth = Physics2D.CircleCast(nextTile, 0.4f, Vector2.up, 1.0f);
            RaycastHit2D raycastSouth = Physics2D.CircleCast(nextTile, 0.4f, Vector2.down, 1.0f);
            RaycastHit2D raycastEast = Physics2D.CircleCast(nextTile, 0.4f, Vector2.right, 1.0f);
            RaycastHit2D raycastWest = Physics2D.CircleCast(nextTile, 0.4f, Vector2.left, 1.0f);

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
                    Debug.DrawLine(nextTile, possibleDirections[i], Color.red);
                    if ((playerPosition - possibleDirections[i]).magnitude < (playerPosition - bestChoice).magnitude)
                    {
                        bestChoice = possibleDirections[i];
                    }
                }
            }
            else
            {
                return lastDirection;
            }

            Debug.DrawLine(nextTile, bestChoice, Color.blue);
            lastDirection = GetDirectionFromVector((bestChoice - nextTile).normalized);
        }
        
        return lastDirection;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.SetState(States.DEATH);

        if (collision.collider.GetComponent<Pacman>())
        {
            collision.collider.GetComponent<Pacman>().Die();
        }
    }
}
