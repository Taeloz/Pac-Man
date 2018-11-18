using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    private int pelletValue = 10;

    private bool isSuperPellet = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAsSuperPellet()
    {
        isSuperPellet = true;
        pelletValue = 50;
        transform.localScale = new Vector3(3, 3, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Pacman>())
        {
            if (isSuperPellet)
            {
                GameManager.Instance.TriggerFleeMode();
            }
            GameManager.Instance.IncrementScore(pelletValue);
            GameManager.Instance.IncrementPellets();
            Destroy(gameObject);
        }
    }
}
