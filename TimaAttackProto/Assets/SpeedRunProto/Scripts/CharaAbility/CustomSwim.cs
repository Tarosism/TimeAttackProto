using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class CustomSwim : CharacterSwim
{
    public override void EnterWater()
    {
        base.EnterWater();
        _controller.CollisionsOff();
        _health.enabled = false;
    }

    public override void ExitWater()
    {
        base.ExitWater();
        _controller.CollisionsOn();
        _health.enabled = true;
    }
}
