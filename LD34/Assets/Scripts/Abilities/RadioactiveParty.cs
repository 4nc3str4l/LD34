using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RadioactiveParty : Ability
{
    public override float CooldownTime
    {
        get
        {
            return 10.0f;
        }
    }

    public override float DestroyTime
    {
        get
        {
            return 10.0f;
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
            return "Radioactive Party";
        }
    }

    public override AbilityType Type
    {
        get
        {
            return AbilityType.RADIOACTIVE_PARTY;
        }
    }
    
    private const float ACCELERATION_FACTOR = 3f;
    private List<GameObject> _sparksPool = new List<GameObject>();

    public RadioactiveParty(AbilityController controller, GameObject owner) : base(controller, owner)
    {
        // Hacky hack:
        _prefabs = controller.Prefabs(AbilityType.RADIOACTIVE_SPARK);

        generatePool();
    }

    private void generatePool(Action<GameObject, bool> callback)
    {
        int count = _sparksPool.Count - 1;

        for (int i = 0; i < 20; ++i)
        {
            bool isNew = false;
            GameObject spark;

            if (i > count)
            {
                spark = GameObject.Instantiate(_prefabs[0]);
                _sparksPool.Add(spark);
                isNew = true;
            }
            else
            {
                spark = _sparksPool[i];
            }

            spark.SetActive(false);
            callback(spark, isNew);
        }
    }

    private void generatePool()
    {
        generatePool((x, y) => { });
    }

    public override void OnStart()
    {
        base.OnStart();

        generatePool((spark, isNew) =>
        {
            spark.SetActive(true);
            spark.transform.position = Owner.transform.position;

            Vector3 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (dest - spark.transform.position).x > 0 ? Vector2.left : Vector2.right;

            MobMovement movement = spark.GetComponent<MobMovement>();
            movement.InitialForce.x = UnityEngine.Random.Range(500, 1000);
            if (UnityEngine.Random.Range(0, 100) >= 50)
            {
                movement.InitialForce = -movement.InitialForce;
            }
            movement.InitialForce.y = UnityEngine.Random.Range(-500, 0);

            if (!isNew)
            {
                movement.ApplyInitialForces();
            }

            movement.ForceJump();

            setupGameobject<SparkPartyDamager>(spark);
        });
    }

    public override void OnEnd()
    {
        generatePool();
    }
}

public class SparkPartyDamager : AbilityDamager
{
    protected override void DoDamage(Mob mob)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Abilities/FireExplosion"), transform.position, Quaternion.identity);
    }
}
