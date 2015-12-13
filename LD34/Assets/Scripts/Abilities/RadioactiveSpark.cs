using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RadioactiveSpark : Ability
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
            return "Radioactive Spark";
        }
    }

    public override AbilityType Type
    {
        get
        {
            return AbilityType.RADIOACTIVE_SPARK;
        }
    }
    
    private const float ACCELERATION_FACTOR = 3f;

    public RadioactiveSpark(AbilityController controller, GameObject owner) : base(controller, owner)
    { }

    public override void OnStart()
    {
        base.OnStart();

        GameObject spark = GameObject.Instantiate(_prefabs[0]);
        spark.transform.position = Owner.transform.position;

        float acceleration = MobMovement.POSITIVE_X_ACCELERATION * ACCELERATION_FACTOR;
        acceleration = UnityEngine.Random.Range(-acceleration, acceleration);

        Vector2 force = new Vector2(acceleration, 0);
        spark.GetComponent<Rigidbody2D>().AddForce(force);   
    }
}



