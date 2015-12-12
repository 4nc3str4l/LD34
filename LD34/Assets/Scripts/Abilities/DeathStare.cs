using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DeathStare : Ability
{
    public override float CooldownTime
    {
        get
        {
            return 1.0f;
        }
    }

    public override float DestroyTime
    {
        get
        {
            return 0.8f;
        }
    }

    public override string Name
    {
        get
        {
            return "Death Stare";
        }
    }

    public override AbilityType Type
    {
        get
        {
            return AbilityType.DEATH_STARE;
        }
    }

    class Ray
    {
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Direction;
        public bool BeingCast;
        public float Distance;
        public GameObject Laser;
    }

    private const float ENLARGE_AMOUNT = 20f;
    private const float SHORTEN_AMOUNT = 40f;

    private bool _active = false;
    private GameObject _startRef;
    private List<Ray> _lasers = new List<Ray>();
    private Vector2 _lastDirection = Vector2.zero;

    public DeathStare(AbilityController controller, GameObject owner) : base(controller, owner)
    {
        _startRef = GameObject.Find("LaserRef");
    }

    public override void OnStart()
    {
        base.OnStart();

        _active = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();

        _active = false;

        _lasers.ForEach(laser => laser.BeingCast = false);
        _lastDirection = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        if (_active)
        {
            Vector2 direction = Owner.transform.rotation.y == 0 ? Vector2.right : Vector2.left;
            if (_lastDirection != direction)
            {
                _lastDirection = direction;

                if (_lasers.Count > 0)
                {
                    _lasers.Last().BeingCast = false;
                }

                GameObject laserHolder = GameObject.Instantiate(_prefabs[1]);
                laserHolder.transform.rotation = Quaternion.Euler(0, direction == Vector2.left ? 0 : 180f, 0);

                _lasers.Add(new Ray()
                {
                    Start = _startRef.transform.position,
                    End = _startRef.transform.position,
                    Direction = direction,
                    BeingCast = true,
                    Laser = laserHolder,
                });
            }
        }

        _lasers.ToList().ForEach(laser =>
        {
            UpdateRay(laser);

            if (!laser.BeingCast && (laser.End - laser.Start).normalized != laser.Direction)
            {
                GameObject.Destroy(laser.Laser);
                _lasers.Remove(laser);
            }
            else
            {
                Vector3 pos = new Vector3(laser.Start.x, laser.Start.y, 0.1f);
                laser.Laser.transform.position = pos;
                laser.Laser.transform.localScale = new Vector3(laser.Distance / 3.5f, 1, 1);
            }
        });
    }

    private void UpdateRay(Ray laser)
    {
        if (!laser.BeingCast)
        {
            laser.Start += laser.Direction * SHORTEN_AMOUNT * Time.deltaTime;
        }
        else
        {
            laser.End += laser.Direction * ENLARGE_AMOUNT * Time.deltaTime;
            laser.Distance = Vector2.Distance(laser.Start, laser.End);

            int mask = ~Constants.Layers.UNIT_MASK & ~Constants.Layers.ABILITIES_MASK;
            RaycastHit2D hit = Physics2D.Raycast(Owner.transform.position, laser.Direction, laser.Distance, mask);
            if (hit.collider != null)
            {
                laser.End = hit.transform.position;
            }
        }

        laser.Distance = Vector2.Distance(laser.Start, laser.End);
    }
}



