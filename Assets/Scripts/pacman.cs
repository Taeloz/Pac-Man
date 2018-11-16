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

public class Pacman : MonoBehaviour
{
    private Directions currentDirection;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
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
        // Move pac-man in the current direction
        switch (currentDirection)
        {
            case Directions.WEST:
                rb.MovePosition(rb.position + Vector2.left * 0.1f);
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Directions.EAST:
                rb.MovePosition(rb.position + Vector2.right * 0.1f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Directions.NORTH:
                rb.MovePosition(rb.position + Vector2.up * 0.1f);
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Directions.SOUTH:
                rb.MovePosition(rb.position + Vector2.down * 0.1f);
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
        }
    }

    void CheckForDirectionChange()
    {
        Directions aimDirection = InputManager.Instance.GetLastInput();

        switch (aimDirection)
        {
            case Directions.NORTH:
                if (Physics2D.Raycast(rb.position + Vector2.left * 0.4f, Vector2.up, 0.6f).collider == null &&
                    Physics2D.Raycast(rb.position + Vector2.right * 0.4f, Vector2.up, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
                    float adjustY = rb.position.y;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;
                }
                break;
            case Directions.SOUTH:
                if (Physics2D.Raycast(rb.position + Vector2.left * 0.4f, Vector2.down, 0.6f).collider == null &&
                    Physics2D.Raycast(rb.position + Vector2.right * 0.4f, Vector2.down, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
                    float adjustY = rb.position.y;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;
                }
                break;
            case Directions.EAST:
                if (Physics2D.Raycast(rb.position + Vector2.up * 0.4f, Vector2.right, 0.6f).collider == null &&
                    Physics2D.Raycast(rb.position + Vector2.down * 0.4f, Vector2.right, 0.6f).collider == null)
                {
                    currentDirection = aimDirection;

                    float adjustX = rb.position.x;
                    float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
                    Vector2 adjustPosition = new Vector2(adjustX, adjustY);
                    rb.position = adjustPosition;
                }
                break;
            case Directions.WEST:
                if (Physics2D.Raycast(rb.position + Vector2.up * 0.4f, Vector2.left, 0.6f).collider == null &&
                    Physics2D.Raycast(rb.position + Vector2.down * 0.4f, Vector2.left, 0.6f).collider == null)
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
        RaycastHit2D raycast1;
        RaycastHit2D raycast2;

        if (currentDirection == Directions.WEST)
        {
            raycast1 = Physics2D.Raycast(rb.position + Vector2.up * 0.4f, Vector2.left, 0.6f);
            raycast2 = Physics2D.Raycast(rb.position + Vector2.down * 0.4f, Vector2.left, 0.6f);
        }
        else if (currentDirection == Directions.EAST)
        {
            raycast1 = Physics2D.Raycast(rb.position + Vector2.up * 0.4f, Vector2.right, 0.6f);
            raycast2 = Physics2D.Raycast(rb.position + Vector2.down * 0.4f, Vector2.right, 0.6f);
        }
        else if (currentDirection == Directions.NORTH)
        {
            raycast1 = Physics2D.Raycast(rb.position + Vector2.left * 0.4f, Vector2.up, 0.6f);
            raycast2 = Physics2D.Raycast(rb.position + Vector2.right * 0.4f, Vector2.up, 0.6f);
        }
        else
        {
            raycast1 = Physics2D.Raycast(rb.position + Vector2.left * 0.4f, Vector2.down, 0.6f);
            raycast2 = Physics2D.Raycast(rb.position + Vector2.right * 0.4f, Vector2.down, 0.6f);
        }

        if (raycast1.collider != null && raycast2.collider != null)
        {
            currentDirection = Directions.STOPPED;

            float adjustX = Mathf.Round(transform.position.x * 2) / 2.0f;
            float adjustY = Mathf.Round(transform.position.y * 2) / 2.0f;
            Vector2 adjustPosition = new Vector2(adjustX, adjustY);
            rb.position = adjustPosition;
        }
    }
}
