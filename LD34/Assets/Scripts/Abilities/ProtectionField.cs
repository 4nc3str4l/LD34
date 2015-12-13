using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ProtectionField : Ability
{
    public override float CooldownTime
    {
        get
        {
            return 4.0f;
        }
    }

    public override float DestroyTime
    {
        get
        {
            return 4.0f;
        }
    }

    public override float DamageInterval
    {
        get
        {
            return 1.0f;
        }
    }

    public override float DamagePerHit
    {
        get
        {
            return UnityEngine.Random.Range(40, 60);
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
            return AbilityType.PROTECTION_FIELD;
        }
    }

    private const int NUMBER_OF_FLAMES = 10;
    private const float ACCELERATION_FACTOR = 3f;
    private List<GameObject> _flames = new List<GameObject>();
    private MonsterFX _monsterFx;

    public ProtectionField(AbilityController controller, GameObject owner) : 
        base(controller, owner)
    {
        _monsterFx = owner.GetComponent<MonsterFX>();
    }

    public override void OnStart()
    {
        base.OnStart();
        _monsterFx.toggleAura();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _monsterFx.toggleAura();
    }
}



