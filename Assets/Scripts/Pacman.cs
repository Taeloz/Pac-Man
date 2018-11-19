using System.Collections;
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
        GameManager.Instance.SetState(States.DEATH);
        GetComponent<Animator>().speed = 1;
        GetComponent<Animator>().SetTrigger("die");

        Invoke("CallLoseLife", 2.0f);
    }

    private void CallLoseLife()
    {
        GameManager.Instance.LoseLife();
    }
}
