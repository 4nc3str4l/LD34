using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HumanGrinder : Ability
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
            return 0.1f;
        }
    }

    public override float DamagePerHit
    {
        get
        {
            return 100f;
        }
    }

    public override string Name
    {
        get
        {
            return "Human Grinder";
        }
    }

    public override AbilityType Type
    {
        get
        {
            return AbilityType.HUMAN_GRINDER;
        }
    }

    private GameObject _blade;

    public HumanGrinder(AbilityController controller, GameObject owner) : base(controller, owner)
    { }

    public override void OnStart()
    {
        base.OnStart();

        _blade = GameObject.Instantiate(_prefabs[0]);
        _blade.transform.position = Owner.transform.position;

        setupGameobject<BladeUpdater>(_blade);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        GameObject.Destroy(_blade);
    }
}

public class BladeUpdater : AbilityDamager
{
    private const float MOVE_SPEED = 4f;

    void Update()
    {
        Vector2 mouseCoordinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 moveDirection = (mouseCoordinates - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * MOVE_SPEED * Time.deltaTime;
        Vector2 newDirection = (mouseCoordinates - newPosition).normalized;

        if (moveDirection != newDirection)
        {
            newPosition = mouseCoordinates;
        }

        transform.position = newPosition;
    }
}
