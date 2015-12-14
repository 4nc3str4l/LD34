using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoulFireTargetted : Ability
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
            return AbilityType.SOUL_FIRE_TARGETED;
        }
    }

    private const int NUMBER_OF_FLAMES = 10;
    private const float ACCELERATION_FACTOR = 1f;
    private List<GameObject> _flames = new List<GameObject>();

    public SoulFireTargetted(AbilityController controller, GameObject owner) : base(controller, owner)
    {
        _prefabs = controller.Prefabs(AbilityType.SOUL_FIRE);
    }

    public override void OnStart()
    {
        base.OnStart();
        
        for (int i = 0; i < NUMBER_OF_FLAMES; ++i)
        {
            GameObject flame = (GameObject)GameObject.Instantiate(_prefabs[0]);
            flame.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float acceleration = MobMovement.POSITIVE_X_ACCELERATION * ACCELERATION_FACTOR;
            acceleration = UnityEngine.Random.Range(-acceleration, acceleration);

            Vector2 force = new Vector2(acceleration, -50f);
            flame.GetComponent<Rigidbody2D>().AddForce(force);

            setupGameobject(flame);

            _flames.Add(flame);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        _flames.ForEach(flame => GameObject.Destroy(flame));
    }
}



