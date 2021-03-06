﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : Sprite
{
    public override void Start()
    {
        base.Start();
    }

    protected override Directions GetMovementDirection()
    {
        return InputManager.Instance.GetLastInput();
    }

    protected override void MoveInCurrentDirection()
    {
        base.MoveInCurrentDirection();

        if (currentDirection != Directions.STOPPED)
        {
            int zRot = 0;

            // Rotate the sprite to match the direction of motion
            switch (currentDirection)
            {
                case Directions.WEST:
                    zRot = 180;
                    break;
                case Directions.EAST:
                    zRot = 0;
                    break;
                case Directions.NORTH:
                    zRot = 90;
                    break;
                case Directions.SOUTH:
                    zRot = 270;
                    break;
            }

            transform.rotation = Quaternion.Euler(0, 0, zRot);
        }
    }

    public void Die()
    {
        GetComponent<Animator>().speed = 1;
        GetComponent<Animator>().SetTrigger("die");

        SoundManager.Instance.PlayDeathClip();

        Invoke("CallLoseLife", 1.0f);
    }

    private void CallLoseLife()
    {
        GameManager.Instance.LoseLife();
    }
}
