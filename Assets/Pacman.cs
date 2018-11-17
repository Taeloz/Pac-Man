using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : Sprite
{
    public override void Start()
    {
        base.Start();
    }

    public override Directions GetMovementDirection()
    {
        return InputManager.Instance.GetLastInput();
    }
}
