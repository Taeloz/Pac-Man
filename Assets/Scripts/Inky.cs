using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Sprite
{
    public LayerMask raycastLayerMask;

    public SpriteRenderer currentEyes;
    public UnityEngine.Sprite[] allEyes;

    private Pacman player;
    private Blinky blinky;
    private Vector2 scatterTarget = new Vector2(13.5f, -2.5f);

    private Directions lastDirection;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
        blinky = FindObjectOfType<Blinky>();
        speed = 2.5f;
        normalSpeed = speed;
        lastMotion = Directions.WEST;
        waiting = true;
    }

    public override Directions GetMovementDirection()
    {
        if (!waitingToChangeDirection)
        {
            Vector2 targetPosition;

            if (returningToSpawn)
            {
                targetPosition = new Vector2(6.5f, 7.5f);
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

            Vector2 movementDir = GetVectorFromDirection(currentDirection);

            // Find the next upcoming potential intersection
            Vector2 nextTile;
            nextTile.x = Mathf.Round((rb.position.x) * 2.0f) / 2.0f + movementDir.x * 0.5f;
            nextTile.y = Mathf.Round((rb.position.y) * 2.0f) / 2.0f + movementDir.y * 0.5f;

            // Check all directions from that intersection
            RaycastHit2D raycastNorth = Physics2D.CircleCast(nextTile, 0.3f, Vector2.up, 0.5f, raycastLayerMask);
            RaycastHit2D raycastSouth = Physics2D.CircleCast(nextTile, 0.3f, Vector2.down, 0.5f, raycastLayerMask);
            RaycastHit2D raycastEast = Physics2D.CircleCast(nextTile, 0.3f, Vector2.right, 0.5f, raycastLayerMask);
            RaycastHit2D raycastWest = Physics2D.CircleCast(nextTile, 0.3f, Vector2.left, 0.5f, raycastLayerMask);

            // Get a list of all the possible directions from that intersection
            List<Vector2> possibleDirections = new List<Vector2>();

            if (raycastWest.collider == null && currentDirection != Directions.EAST && lastMotion != Directions.EAST)
            {
                possibleDirections.Add(nextTile + Vector2.left * 0.5f);
            }
            if (raycastNorth.collider == null && currentDirection != Directions.SOUTH && lastMotion != Directions.SOUTH)
            {
                possibleDirections.Add(nextTile + Vector2.up * 0.5f);
            }
            if (raycastEast.collider == null && currentDirection != Directions.WEST && lastMotion != Directions.WEST)
            {
                possibleDirections.Add(nextTile + Vector2.right * 0.5f);
            }
            if (raycastSouth.collider == null && currentDirection != Directions.NORTH && lastMotion != Directions.NORTH)
            {
                possibleDirections.Add(nextTile + Vector2.down * 0.5f);
            }

            // If there is at least one possible direction, check for the best one
            Vector2 bestChoice;

            if (possibleDirections.Count > 0)
            {
                waitingToChangeDirection = true;

                if (GameManager.Instance.gameState == States.FLEE)
                {
                    int randomChoice = Random.Range(0, possibleDirections.Count);
                    bestChoice = possibleDirections[randomChoice];
                }
                else
                {
                    bestChoice = possibleDirections[0];

                    for (int i = 0; i < possibleDirections.Count; i++)
                    {
                        if ((targetPosition - possibleDirections[i]).magnitude < (targetPosition - bestChoice).magnitude)
                        {
                            bestChoice = possibleDirections[i];
                        }
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
        if (collision.collider.GetComponent<Pacman>() && !returningToSpawn)
        {
            if (GameManager.Instance.gameState == States.FLEE)
            {
                Eaten();
            }
            else
            {
                GameManager.Instance.SetState(States.DEATH);
                collision.collider.GetComponent<Pacman>().Die();
            }

        }
    }
}
