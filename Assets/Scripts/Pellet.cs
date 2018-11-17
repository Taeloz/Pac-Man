using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    private const int pelletValue = 10;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Pacman>())
        {
            GameManager.Instance.IncrementScore(pelletValue);
            GameManager.Instance.IncrementPellets();
            Destroy(gameObject);
        }
    }
}
