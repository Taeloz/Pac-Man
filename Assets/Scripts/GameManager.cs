using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum States
{
    INTRO,
    CHASE,
    SCATTER,
    FLEE,
    DEATH,
    GAMEOVER
};

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private List<Sprite> ghosts;
    private GameObject player;

    private int score = 0;
    private int pelletsEaten = 0;
    private int totalPellets = 0;
    private int lives = 3;
    private Text scoreText;
    private Text readyText;
    private List<Image> lifeImages;

    private bool ghostSpawnerActive;

    private float roundTimer;
    private float fleeTimer;

    public States gameState;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObject = new GameObject("GameManager");
                instance = managerObject.AddComponent<GameManager>();
            }

            return instance;
        }
    }


    private void Awake()
    {
        roundTimer = 0;
        score = 0;
        gameState = States.INTRO;
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        readyText = GameObject.FindGameObjectWithTag("ReadyText").GetComponent<Text>();

        player = GameObject.FindGameObjectWithTag("Player");

        ghosts = new List<Sprite>
        {
            FindObjectOfType<Blinky>(),
            FindObjectOfType<Pinky>(),
            FindObjectOfType<Inky>(),
            FindObjectOfType<Clyde>()
        };

        lifeImages = new List<Image>
        {
            GameObject.FindGameObjectWithTag("life1").GetComponent<Image>(),
            GameObject.FindGameObjectWithTag("life2").GetComponent<Image>(),
            GameObject.FindGameObjectWithTag("life3").GetComponent<Image>()
        };
    }

    private void Update()
    {
        roundTimer += Time.deltaTime;

        // Take care of the opening
        if (gameState == States.INTRO && roundTimer >= 2)
        {
            gameState = States.CHASE;
            readyText.enabled = false;
            ghostSpawnerActive = true;
            StartCoroutine(GhostSpawner());
        }

        if (gameState == States.FLEE)
        {
            fleeTimer -= Time.deltaTime;

            if (fleeTimer <= 0)
            {
                UnTriggerFleeMode();
            }
        }

        // Increase Blinky's speed
        if (pelletsEaten >= 40)
        {
            ghosts[0].SetSpeed(3.0f);
        }
        if (pelletsEaten >= totalPellets / 3)
        {
            ghosts[0].SetSpeed(3.55f);
        }
        
    }

    public void IncrementScore(int inc)
    {
        score += inc;
        scoreText.text = "Score : " + score;
    }

    public void IncrementPellets()
    {
        pelletsEaten++;
    }

    public void IncrementTotalPellets()
    {
        totalPellets++;
    }

    public void SetState(States newState)
    {
        gameState = newState;
    }

    public void TriggerFleeMode()
    {
        gameState = States.FLEE;

        fleeTimer = 10.0f;

        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].SetFlee();
        }
    }

    public void UnTriggerFleeMode()
    {
        gameState = States.CHASE;

        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].UnsetFlee();
        }
    }

    public void LoseLife()
    {
        lives--;
        switch (lives)
        {
            case 2:
                lifeImages[2].enabled = false;
                break;
            case 1:
                lifeImages[1].enabled = false;
                break;
            case 0:
                lifeImages[0].enabled = false;
                break;
            case -1:
                //GameOver
                break;
        }

        ghostSpawnerActive = false;
        ResetPositions();
        player.GetComponent<Animator>().SetTrigger("reset");
        player.GetComponent<Sprite>().waiting = false;

        roundTimer = 0;
        gameState = States.INTRO;
        readyText.enabled = true;

    }

    public void ResetPositions()
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].ResetPosition();
        }

        player.GetComponent<Sprite>().ResetPosition();
        player.GetComponent<Sprite>().transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
    }

    private IEnumerator GhostSpawner()
    {
        while(ghostSpawnerActive)
        {
            if (ghosts[0].waiting)
            {
                ghosts[0].waiting = false;
                ghosts[0].spawning = true;
            }
            else if (ghosts[1].waiting && Time.time >= 4)
            {
                ghosts[1].waiting = false;
                ghosts[1].spawning = true;
            }
            else if (ghosts[2].waiting && pelletsEaten >= 40)
            {
                ghosts[2].waiting = false;
                ghosts[2].spawning = true;
            }
            else if (ghosts[3].waiting && pelletsEaten >= totalPellets / 3)
            {
                ghosts[3].waiting = false;
                ghosts[3].spawning = true;
            }

            yield return new WaitForSeconds(2.0f);
        }
        
    }

}
