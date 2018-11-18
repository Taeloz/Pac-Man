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
    protected Color startColor;

    protected float speed = 3.5f;
    protected float normalSpeed;
    protected Vector2 startPosition;

    protected bool waitingToChangeDirection = false;
    protected bool returningToSpawn = false;

    public bool waiting = false;
    public bool spawning = false;


    // Use this for initialization
    public virtual void Start()
    {
        currentDirection = Directions.WEST;
        rb = GetComponent<Rigidbody2D>();
        startColor = GetComponent<SpriteRenderer>().color;
        startPosition = rb.transform.position;
        normalSpeed = speed;
    }

    public void ResetPosition()
    {
        rb.transform.position = startPosition;
        currentDirection = Directions.WEST;
        waiting = true;
        spawning = false;
        returningToSpawn = false;
        waitingToChangeDirection = false;
        speed = normalSpeed;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().color = startColor;

    }

    public virtual Directions GetMovementDirection()
    {
        return Directions.WEST;
    }

    public Directions GetCurrentDirection()
    {
        return currentDirection;
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
        if (!returningToSpawn)
        {
            ReverseDirection();
            GetComponent<SpriteRenderer>().color = new Color32(0, 50, 150, 255);
        }
        
    }

    public void UnsetFlee()
    {
        if (!returningToSpawn)
        {
            ReverseDirection();
            GetComponent<SpriteRenderer>().color = startColor;
        }        
    }


    public void ReverseDirection()
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

    public void Eaten()
    {
        returningToSpawn = true;
        normalSpeed = speed;
        speed = 6f;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        if (GameManager.Instance.gameState == States.CHASE || GameManager.Instance.gameState == States.SCATTER || GameManager.Instance.gameState == States.FLEE)
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
            if (GameManager.Instance.gameState == States.CHASE || GameManager.Instance.gameState == States.SCATTER || GameManager.Instance.gameState == States.FLEE)
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
        
    }

    public virtual void MoveInCurrentDirection()
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

    void CheckForDirectionChange()
    {
        Directions aimDirection = GetMovementDirection();

        Collider2D hit;

        // If the new direction is possible, re-align sprite to grid and change direction
        switch (aimDirection)
        {
            case Directions.NORTH:
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.up, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>())
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
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.down, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>())
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
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.right, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>())
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
                hit = Physics2D.CircleCast(rb.position, 0.3f, Vector2.left, 0.6f).collider;
                if (hit == null || hit.GetComponent<Sprite>())
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

        RaycastHit2D raycast = Physics2D.CircleCast(rb.position, 0.3f, rayDir, 0.3f);

        if (raycast.collider != null && !raycast.collider.GetComponent<Sprite>())
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
