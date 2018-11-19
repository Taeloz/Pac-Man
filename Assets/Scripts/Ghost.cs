using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ghost : Sprite
{
    public LayerMask raycastLayerMask;

    public SpriteRenderer currentEyes;
    public UnityEngine.Sprite[] allEyes;

    protected float normalSpeed;
    protected Vector2 scatterTarget;
    protected Color startColor;
    protected Directions lastDirection;

    public bool waiting;
    public bool spawning;
    public bool fleeing;
    protected bool returningToSpawn = false;

    protected abstract Vector2 GetTargetPosition();

    public override void Start()
    {
        base.Start();

        startColor = GetComponent<SpriteRenderer>().color;

        lastMotion = Directions.WEST;
        fleeing = false;
    }

    public override void FixedUpdate()
    {
        // If the ghost is spawning, move it upwards out of the holding cell
        if (spawning)
        {
            float x = 6.5f;
            float y = transform.position.y + 0.05f;
            transform.position = new Vector2(x, y);
            if (y >= 9.0f)
            {
                transform.position = new Vector2(x, 9.0f);
                spawning = false;
            }
        }

        // If the ghost is returning to the spawn, check to see if it has reached it
        if (returningToSpawn)
        {
            if (Mathf.Abs(rb.position.x - 6.5f) <= 0.05f &&
                Mathf.Abs(rb.position.y - 9.0f) <= 0.05f)
            {
                returningToSpawn = false;
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<SpriteRenderer>().color = startColor;
                transform.position = new Vector2(6.5f, 7.5f);
                waiting = true;
                speed = normalSpeed;
            }
        }

        if (!waiting && !spawning)
        {
            base.FixedUpdate();
        }
    }

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

    public override void ResetPosition()
    {
        base.ResetPosition();

        speed = normalSpeed;
        waiting = true;
        spawning = false;
        returningToSpawn = false;
        startColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().color = startColor;
    }

    /// <summary>
    /// Given a target position and the list of possible directions, return the one that is closest to the target
    /// </summary>
    /// <param name="targetPosition">The target position</param>
    /// <param name="possibleDirections">The possible directions to choose from</param>
    /// <returns></returns>
    protected Vector2 ChooseBestDirection(Vector2 targetPosition, List<Vector2> possibleDirections)
    {
        if (possibleDirections.Count < 1)
        {
            Debug.LogError("Trying to choose from a list of possible directions of size 0");
            return Vector2.zero;
        }

        waitingToChangeDirection = true;

        Vector2 bestChoice;

        if (fleeing)
        {
            int randomChoice = Random.Range(0, possibleDirections.Count);
            bestChoice = possibleDirections[randomChoice];
        }
        else
        {
            bestChoice = possibleDirections[0];

            for (int i = 0; i < possibleDirections.Count; i++)
            {
                if ( (targetPosition - possibleDirections[i]).sqrMagnitude < (targetPosition - bestChoice).sqrMagnitude)
                {
                    bestChoice = possibleDirections[i];
                }
            }
        }

        return bestChoice;
    }

    protected override Directions GetMovementDirection()
    {
        if (!waitingToChangeDirection)
        {
            Vector2 targetPosition = GetTargetPosition();            

            Vector2 movementDir = GetVectorFromDirection(currentDirection);

            // Find the next upcoming potential intersection
            Vector2 nextTile;
            nextTile.x = Mathf.Round((rb.position.x) * 2.0f) / 2.0f + movementDir.x * 0.5f;
            nextTile.y = Mathf.Round((rb.position.y) * 2.0f) / 2.0f + movementDir.y * 0.5f;

            // Get a list of possible directions from that intersection
            List<Vector2> possibleDirections = GetPossibleDirections(nextTile);

            // If there is at least one possible direction, check for the best one
            Vector2 bestChoice;

            if (possibleDirections.Count > 0)
            {
                bestChoice = ChooseBestDirection(targetPosition, possibleDirections);
            }
            else
            {
                return lastDirection;
            }

            lastDirection = GetDirectionFromVector((bestChoice - nextTile).normalized);
        }

        return lastDirection;
    }

    /// <summary>
    /// Get a list of possible directions from a given position
    /// </summary>
    /// <param name="nextTile">The position to check from</param>
    /// <returns></returns>
    protected List<Vector2> GetPossibleDirections(Vector2 nextTile)
    {
        // Check all directions from that intersection
        RaycastHit2D raycastNorth = Physics2D.CircleCast(nextTile, 0.2f, Vector2.up, 1.0f, raycastLayerMask);
        RaycastHit2D raycastSouth = Physics2D.CircleCast(nextTile, 0.2f, Vector2.down, 1.0f, raycastLayerMask);
        RaycastHit2D raycastEast = Physics2D.CircleCast(nextTile, 0.2f, Vector2.right, 1.0f, raycastLayerMask);
        RaycastHit2D raycastWest = Physics2D.CircleCast(nextTile, 0.2f, Vector2.left, 1.0f, raycastLayerMask);

        // Get a list of all the possible directions from that intersection
        List<Vector2> possibleDirections = new List<Vector2>();

        // Add the possible directions to the list
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

        return possibleDirections;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Pacman>() && !returningToSpawn)
        {
            if (GameManager.Instance.gameState == States.FLEE && fleeing)
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

    protected void Eaten()
    {
        returningToSpawn = true;
        fleeing = false;
        normalSpeed = speed;
        speed = 8f;
        GetComponent<SpriteRenderer>().enabled = false;
        GameManager.Instance.IncrementScore(200);
        SoundManager.Instance.PlayEatGhost();
    }

    public void SetSpeed(float newSpeed)
    {
        if (!returningToSpawn)
        {
            speed = newSpeed;
            normalSpeed = speed;
        }
        else
        {
            normalSpeed = newSpeed;
        }
    }

    public void SetFlee()
    {
        if (!returningToSpawn && !waiting && !spawning)
        {
            ReverseDirection();
            GetComponent<SpriteRenderer>().color = new Color32(0, 50, 150, 255);
            normalSpeed = speed;
            speed = 2.0f;
            fleeing = true;
        }
    }

    public void UnsetFlee()
    {
        if (!returningToSpawn)
        {
            ReverseDirection();
            GetComponent<SpriteRenderer>().color = startColor;
            speed = normalSpeed;
            fleeing = false;
        }
    }

    protected void ReverseDirection()
    {
        waitingToChangeDirection = false;

        switch (currentDirection)
        {
            case Directions.EAST:
                currentDirection = Directions.WEST;
                lastMotion = Directions.WEST;
                break;
            case Directions.NORTH:
                currentDirection = Directions.SOUTH;
                lastMotion = Directions.SOUTH;
                break;
            case Directions.SOUTH:
                currentDirection = Directions.NORTH;
                lastMotion = Directions.NORTH;
                break;
            case Directions.WEST:
                currentDirection = Directions.EAST;
                lastMotion = Directions.EAST;
                break;
        }
    }
}
