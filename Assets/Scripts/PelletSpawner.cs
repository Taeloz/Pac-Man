using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletSpawner : MonoBehaviour
{
    public Pellet pellet;

    // Use this for initialization
    void Start()
    {
        SpawnPellets();
    }

    private void SpawnPellets()
    {
        for (float i = 0; i <= 14; i += 0.5f)
        {
            for (float j = 0; j <= 13; j += 0.5f)
            {
                // Don't spawn in the center, or near the warp tunnel
                if (i >= 5 && i <= 10 && ((j >= 3 && j <= 10) || (j <= 2) || (j >= 11)))
                {
                    continue;
                }

                Vector2 pos = new Vector2(j, i);

                // Don't spawn if intersecting a wall
                if (Physics2D.OverlapCircle(pos, 0.3f) != null)
                {
                    continue;
                }

                GameManager.Instance.IncrementTotalPellets();
                Pellet thisPellet = Instantiate(pellet, pos, Quaternion.identity);
                if (i == 3 || i == 13)
                {
                    if (j == 0 || j == 13)
                    {
                        thisPellet.SetAsSuperPellet();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
