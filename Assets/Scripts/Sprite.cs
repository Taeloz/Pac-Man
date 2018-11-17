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
    private Directions currentDirection;
    private Rigidbody2D rb;

    private const float speed = 0.1f;

    // Use this for initialization
    public virtual void Start()
    {
        currentDirection = Directions.WEST;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == States.CHASE)
        {
            if (currentDirection != Directions.STOPPED)
            {
                CheckIfStoppedByWall();
            }

            // If pac-man is at a potential grid intersection, check for a direction change
            if (Mathf.Abs(rb.position.x - Mathf.Round(rb.position.x * 2) / 2.0f) <= 0.05f &&
                Mathf.Abs(rb.position.y - Mathf.Round(rb.position.y * 2) / 2.0f) <= 0.05f)
            {
                CheckForDirectionChange();
            }

            MoveInCurrentDirection();
        }
    }

    void MoveInCurrentDirection()
    {
        int zRot = -1;
        Vector2 dir = Vector2.zero;

        // Move pac-man in the current direction
        switch (currentDirection)
        {
            case Directions.WEST:
                dir = Vector2.left;
                zRot = 180;
                break;
            case Directions.EAST:
                dir = Vector2.right;
                zRot = 0;
                break;
            case Directions.NORTH:
                dir = Vector2.up;
                zRot = 90;
                break;
            case Directions.SOUTH:
                dir = Vector2.down;
                zRot = 270;
                break;
        }

        // Don't move if the currentDirection is STOPPED
        if (zRot >= 0)
        {
            rb.MovePosition(rb.position + dir * speed);
            transform.rotation = Quaternion.Euler(0, 0, zRot);
        }
        
    }

    public virtual Directions GetMovementDirection()
    {
        return Directions.WEST;
    }

    void CheckForDirectionChange()
    {
        Directions aimDirection = GetMovementDirection();

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
                }
                break;
        }
    }

    void CheckIfStoppedByWall()
    {
        Vector2 rayDir = Vector2.zero;

        switch(currentDirection)
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

        RaycastHit2D raycast = Physics2D.CircleCast(rb.position, 0.4f, rayDir, 0.3f);

        if (raycast.collider != null)
        {
            currentDirection = Directions.STOPPED;

            float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
            float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
            Vector2 adjustPosition = new Vector2(adjustX, adjustY);
            rb.position = adjustPosition;
        }
    }
}
