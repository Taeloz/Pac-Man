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

    protected float speed = 0.1f;

    protected bool waitingToChangeDirection = false;

    // Use this for initialization
    public virtual void Start()
    {
        currentDirection = Directions.WEST;
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual Directions GetMovementDirection()
    {
        return Directions.WEST;
    }

    void Update()
    {
        if (GameManager.Instance.gameState == States.CHASE)
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
    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == States.CHASE)
        {
            if (currentDirection != Directions.STOPPED)
            {
                CheckIfStoppedByWall();
            }

            // If sprite is at a potential grid intersection, check for a direction change
            if (Mathf.Abs(rb.position.x - Mathf.Round(rb.position.x * 2) / 2.0f) <= 0.005f &&
                Mathf.Abs(rb.position.y - Mathf.Round(rb.position.y * 2) / 2.0f) <= 0.005f)
            {
                CheckForDirectionChange();
            }

            MoveInCurrentDirection();
        }
    }

    public virtual void MoveInCurrentDirection()
    {
        Vector2 dir = GetVectorFromDirection(currentDirection);

        if (currentDirection != Directions.STOPPED)
        {
            rb.MovePosition(rb.position + dir * speed);
        }
                
    }

    void CheckForDirectionChange()
    {
        Directions aimDirection = GetMovementDirection();

        // If the new direction is possible, re-align sprite to grid and change direction
        switch (aimDirection)
        {
            case Directions.NORTH:
                if (Physics2D.CircleCast(rb.position, 0.4f, Vector2.up, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
                    float adjustY = rb.position.y;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;

                    waitingToChangeDirection = false;
                }
                break;
            case Directions.SOUTH:
                if (Physics2D.CircleCast(rb.position, 0.4f, Vector2.down, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
                    float adjustY = rb.position.y;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;

                    waitingToChangeDirection = false;
                }
                break;
            case Directions.EAST:
                if (Physics2D.CircleCast(rb.position, 0.4f, Vector2.right, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = rb.position.x;
                    float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;

                    waitingToChangeDirection = false;
                }
                break;
            case Directions.WEST:
                if (Physics2D.CircleCast(rb.position, 0.4f, Vector2.left, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = rb.position.x;
                    float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;

                    waitingToChangeDirection = false;
                }
                break;
        }
    }

    void CheckIfStoppedByWall()
    {
        Vector2 rayDir = GetVectorFromDirection(currentDirection);

        RaycastHit2D raycast = Physics2D.CircleCast(rb.position, 0.4f, rayDir, 0.1f);

        if (raycast.collider != null)
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
