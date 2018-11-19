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
    GAMEOVER,
    WIN
};

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private List<Ghost> ghosts;
    private GameObject player;

    private int score = 0;
    private int pelletsEaten = 0;
    private int totalPellets = 0;
    private int lives = 3;

    private Text scoreText;
    private Text readyText;
    private Text gameOverText;
    private Text congratsText;
    private List<Image> lifeImages;

    private float roundTimer;
    private float fleeTimer;
    private float waveTimer;

    public States gameState;
    private States previousState;

    private bool ghostSpawnerActive = false;
    private bool wavesActive = false;

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

        gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<Text>();
        congratsText = GameObject.FindGameObjectWithTag("congratsText").GetComponent<Text>();
        gameOverText.enabled = false;
        congratsText.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player");

        ghosts = new List<Ghost>
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
            gameState = States.SCATTER;
            readyText.enabled = false;            
            ghostSpawnerActive = true;
            wavesActive = true;
            waveTimer = 7.0f;
            StartCoroutine(GhostSpawner());
            StartCoroutine(WaveControl());
        }

        if (gameState == States.GAMEOVER)
        {
            wavesActive = false;
            ghostSpawnerActive = false;

            gameOverText.enabled = true;
        }
        else if (gameState == States.WIN)
        {
            wavesActive = false;
            ghostSpawnerActive = false;

            congratsText.enabled = true;
        }
        else if (gameState == States.FLEE)
        {
            fleeTimer -= Time.deltaTime;

            if (fleeTimer <= 0)
            {
                UnTriggerFleeMode();
            }
        } else
        {
            waveTimer -= Time.deltaTime;
        }

        // Increase Blinky's speed based on pellets eaten
        if (pelletsEaten >= 40)
        {
            ghosts[0].SetSpeed(2.75f);
        }
        if (pelletsEaten >= totalPellets / 2)
        {
            ghosts[0].SetSpeed(3.05f);
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
        if (pelletsEaten >= totalPellets)
        {
            gameState = States.WIN;
        }
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
        previousState = gameState;

        gameState = States.FLEE;

        fleeTimer = 7.0f;

        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].SetFlee();
        }
    }

    public void UnTriggerFleeMode()
    {
        gameState = previousState;

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
                gameState = States.GAMEOVER;
                break;
        }

        if (gameState != States.GAMEOVER)
        {
            ghostSpawnerActive = false;
            wavesActive = false;
            ResetPositions();
            player.GetComponent<Animator>().SetTrigger("reset");

            roundTimer = 0;
            gameState = States.INTRO;
            readyText.enabled = true;
        }
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
            else if (ghosts[1].waiting && roundTimer >= 6)
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

            yield return new WaitForSeconds(3.0f);
        }        
    }

    private IEnumerator WaveControl()
    {
        int waveCounter = 0;

        while (wavesActive)
        {            
            if (waveTimer <= 0)
            {
                float nextTime = 0;
                
                if (gameState == States.CHASE && waveCounter < 7)
                {
                    gameState = States.SCATTER;
                    nextTime = waveCounter > 3 ? 5.0f : 7.0f;
                }
                else
                {
                    gameState = States.CHASE;
                    nextTime = 15.0f;
                }
                waveTimer = nextTime;
                waveCounter++;
            }
            yield return new WaitForFixedUpdate();
        }
    }

}
