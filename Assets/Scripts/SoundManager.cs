using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    private AudioSource audioSource;

    public AudioClip pelletClip;
    public AudioClip powerPellet;
    public AudioClip eatGhost;
    public AudioClip death;

    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayPelletClip()
    {
        audioSource.PlayOneShot(pelletClip);
    }

    public void PlayPowerPelletClip()
    {
        audioSource.PlayOneShot(powerPellet);
    }

    public void PlayEatGhost()
    {
        audioSource.PlayOneShot(eatGhost);
    }

    public void PlayDeathClip()
    {
        audioSource.PlayOneShot(death);
    }


    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

}
