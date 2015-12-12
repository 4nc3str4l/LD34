using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoulFire : Ability
{
    public override float CooldownTime
    {
        get
        {
            return 5.0f;
        }
    }

    public override string Name
    {
        get
        {
            return "Soul Fire";
        }
    }

    public override AbilityType Type
    {
        get
        {
            return AbilityType.SOUL_FIRE;
        }
    }

    public SoulFire(GameObject owner) : base(owner)
    { }
}
