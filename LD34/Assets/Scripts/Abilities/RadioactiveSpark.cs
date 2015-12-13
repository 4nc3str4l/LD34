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
            return 2.0f;
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

        Vector3 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (dest - spark.transform.position).x > 0 ? Vector2.left : Vector2.right;

        MobMovement movement = spark.GetComponent<MobMovement>();
        if (direction == Vector2.right)
        {
            movement.InitialForce = -movement.InitialForce;
        }
        movement.ForceJump();

        setupGameobject<SparkDamager>(spark);
    }
}

public class SparkDamager : AbilityDamager
{
    protected override void DoDamage(Mob mob)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Abilities/FireExplosion"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
