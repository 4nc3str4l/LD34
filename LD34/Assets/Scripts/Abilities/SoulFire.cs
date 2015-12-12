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

    private List<GameObject> _flames;

    public SoulFire(AbilityController controller, GameObject owner) : base(controller, owner)
    {
        foreach (GameObject prefab in _prefabs)
        {
            GameObject flame = GameObject.Instantiate(prefab);
            flame.SetActive(false);
            _flames.Add(flame);
        }
    }

    public override void OnStart()
    {
        base.OnStart();

        _flames.ForEach(f => f.SetActive(true));
    }

    public override void OnEnd()
    {
        base.OnEnd();

        _flames.ForEach(f => f.SetActive(false));
    }
}
