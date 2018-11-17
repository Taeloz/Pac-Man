using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Sprite
{
    private Pacman player;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Pacman>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
