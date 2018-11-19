using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    private int pelletValue = 10;

    private bool isSuperPellet = false;

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
                SoundManager.Instance.PlayPowerPelletClip();
            }
            else
            {
                SoundManager.Instance.PlayPelletClip();
            }

            GameManager.Instance.IncrementScore(pelletValue);
            GameManager.Instance.IncrementPellets();
            
            Destroy(gameObject);
        }
    }
}
