using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    NORTH,
    SOUTH,
    EAST,
    WEST,
    STOPPED
};

public class Sprite : MonoBehaviour
{
    protected Directions currentDirection;
    protected Directions lastMotion;
    protected Rigidbody2D rb;

    protected float speed = 3.0f;
    protected Vector2 startPosition;

    protected bool waitingToChangeDirection = false;


    public virtual void Start()
    {
        currentDirection = Directions.WEST;
        rb = GetComponent<Rigidbody2D>();        
        startPosition = rb.transform.position;
    }

    public virtual void ResetPosition()
    {
        rb.transform.position = startPosition;
        currentDirection = Directions.WEST;
        waitingToChangeDirection = false;        
    }

    protected virtual Directions GetMovementDirection()
    {
        return Directions.WEST;
    }

    public Directions GetCurrentDirection()
    {
        return currentDirection;
    }

    void Update()
    {
        if (GameManager.Instance.gameState == States.CHASE ||
            GameManager.Instance.gameState == States.SCATTER ||
            GameManager.Instance.gameState == States.FLEE)
        {
            if (currentDirection == Directions.STOPPED)
            {
                GetComponent<Animator>().speed = 0;
            }
            else
            {
                GetComponent<Animator>().speed = 1;
            }
        }
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {        
        if (GameManager.Instance.gameState == States.CHASE ||
            GameManager.Instance.gameState == States.SCATTER ||
            GameManager.Instance.gameState == States.FLEE)
        {
            if (currentDirection != Directions.STOPPED)
            {
                CheckIfStoppedByWall();
            }

            // If sprite is at a potential grid intersection, check for a direction change
            if (Mathf.Abs(rb.position.x - Mathf.Round(rb.position.x * 2) / 2.0f) <= 0.1f &&
                Mathf.Abs(rb.position.y - Mathf.Round(rb.position.y * 2) / 2.0f) <= 0.1f)
            {
                CheckForDirectionChange();
            }

            MoveInCurrentDirection();
        }
    }

    /// <summary>
    /// Move the sprite in the currently facing direction by its speed
    /// </summary>
    protected virtual void MoveInCurrentDirection()
    {
        Vector2 dir = GetVectorFromDirection(currentDirection);

        if (currentDirection != Directions.STOPPED)
        {
            rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
            if (rb.position.x < -1)
            {
                rb.position = new Vector2(15, rb.position.y);
            }
            else if (rb.position.x > 15)
            {
                rb.position = new Vector2(-1, rb.position.y);
            }
        }                
    }

    /// <summary>
    /// Check if a direction change is desired, and then change direction if it is possible
    /// </summary>
    protected void CheckForDirectionChange()
    {
        Directions aimDirection = GetMovementDirection();

        Collider2D hit;

        // If the new direction is possible, re-align sprite to grid and change direction
        switch (aimDirection)
        {
            case Directions.NORTH:
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.up, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>() != null)
                {
                    float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
                    float adjustY = rb.position.y;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);

                    ChangeDirection(aimDirection, adjustPosition);
                }
                break;
            case Directions.SOUTH:
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.down, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>() != null)
                {
                    float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
                    float adjustY = rb.position.y;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);

                    ChangeDirection(aimDirection, adjustPosition);
                }
                break;
            case Directions.EAST:
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.right, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>() != null)
                {
                    float adjustX = rb.position.x;
                    float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);

                    ChangeDirection(aimDirection, adjustPosition);
                }
                break;
            case Directions.WEST:
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.left, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>() != null)
                {
                    float adjustX = rb.position.x;
                    float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);

                    ChangeDirection(aimDirection, adjustPosition);
                }
                break;
        }
    }

    protected void ChangeDirection(Directions aimDirection, Vector2 adjustPosition)
    {
        currentDirection = aimDirection;
        lastMotion = currentDirection;

        rb.position = adjustPosition;

        waitingToChangeDirection = false;
    }

    void CheckIfStoppedByWall()
    {
        Vector2 rayDir = GetVectorFromDirection(currentDirection);

        RaycastHit2D raycast = Physics2D.CircleCast(rb.position, 0.3f, rayDir, 0.1f);

        if (raycast.collider != null && raycast.collider.GetComponent<Sprite>() == null)
        {
            lastMotion = currentDirection;
            currentDirection = Directions.STOPPED;

            float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
            float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
            Vector2 adjustPosition = new Vector2(adjustX, adjustY);
            rb.position = adjustPosition;

            waitingToChangeDirection = false;
        }
    }

    /// <summary>
    /// Given a cardinal direction, return a unit vector representing it
    /// </summary>
    /// <param name="dir">A cardinal direction</param>
    /// <returns></returns>
    protected Vector2 GetVectorFromDirection(Directions dir)
    {
        Vector2 rayDir = Vector2.zero;

        switch (dir)
        {
            case Directions.NORTH:
                rayDir = Vector2.up;
                break;
            case Directions.SOUTH:
                rayDir = Vector2.down;
                break;
            case Directions.EAST:
                rayDir = Vector2.right;
                break;
            case Directions.WEST:
                rayDir = Vector2.left;
                break;
        }

        return rayDir;
    }

    /// <summary>
    /// Given a vector, return the cardinal direction representing it
    /// </summary>
    /// <param name="dir">A vector representing a cardinal direction</param>
    /// <returns></returns>
    protected Directions GetDirectionFromVector(Vector2 dir)
    {
        Directions result;

        if (dir.y > 0)
        {
            result = Directions.NORTH;
        }
        else if (dir.y < 0)
        {
            result = Directions.SOUTH;
        }
        else if (dir.x > 0)
        {
            result = Directions.EAST;
        }
        else if (dir.x < 0)
        {
            result = Directions.WEST;
        }
        else
        {
            result = Directions.WEST;
        }

        return result;
    }
}
