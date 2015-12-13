using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Madness : Ability
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
            return "Madness";
        }
    }

    public override AbilityType Type
    {
        get
        {
            return AbilityType.MADNESS;
        }
    }

    private const int NUMBER_OF_FLAMES = 10;
    private const float ACCELERATION_FACTOR = 3f;
    private List<GameObject> _flames = new List<GameObject>();
    private Light _pointLight;

    public Madness(AbilityController controller, GameObject owner) : base(controller, owner)
    {
        _pointLight = GameObject.Find("Point light").GetComponent<Light>(); 
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void Update()
    {
        base.Update();

        if (_enabled)
        {
            _pointLight.color = Color.Lerp(_pointLight.color, Color.magenta, 0.1f);
        }
        else
        {
            _pointLight.color = Color.Lerp(_pointLight.color, Color.white, 0.1f);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        _flames.ForEach(flame => GameObject.Destroy(flame));
    }
}



