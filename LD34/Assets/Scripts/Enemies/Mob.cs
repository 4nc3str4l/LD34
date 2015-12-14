﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Flags]
public enum MobFlags
{
    CAN_JUMP            = 1,
    PROXIMITY_PANIC     = 2,
    LOW_HEALTH_PANIC    = 4,
    STRIKE_EVERYONE     = 8,
    ATTACK              = 16
}

public class Mob : Entity
{
    [SerializeField] [EnumFlagsAttribute]
    public MobFlags Flags = MobFlags.CAN_JUMP | MobFlags.PROXIMITY_PANIC | MobFlags.LOW_HEALTH_PANIC;
    public float PanicDistance = 5f;
    public float HealthThreshold = 10f;
    public float AttackDistance = 8f;
    public float AttackRate = 0.5f;
    public Vector2 StrikeDirection = Vector2.right;
    
    private const float JUMP_INTERVAL = 5f;

    private Entity _monster;
    private MobMovement _movement;
    private MonsterFX _monsterFx;
    private Vector2 _panicDirection = Vector2.left;
    private Transform _bulletPosition;
    private float _lastJump;
    private float _lastProximityPanic;
    private float _lastAttack;

    private bool _canJump;
    private bool _canProximityPanicRun;
    private bool _canLowHealthRun;
    private bool _canStrike;
    private bool _canAttack;
    
    public void Start()
    {
        _monster = GameObject.Find("MonsterContainer").GetComponentInChildren<Entity>();
        _monsterFx = transform.GetComponent<MonsterFX>();
        _movement = transform.parent.GetComponent<MobMovement>();

        _canJump = (Flags & MobFlags.CAN_JUMP) == MobFlags.CAN_JUMP;
        _canLowHealthRun = (Flags & MobFlags.LOW_HEALTH_PANIC) == MobFlags.LOW_HEALTH_PANIC;
        _canProximityPanicRun = (Flags & MobFlags.PROXIMITY_PANIC) == MobFlags.PROXIMITY_PANIC;
        _canStrike = (Flags & MobFlags.STRIKE_EVERYONE) == MobFlags.STRIKE_EVERYONE;
        _canAttack = (Flags & MobFlags.ATTACK) == MobFlags.ATTACK;

        _bulletPosition = transform.Find("BulletRef");
    }

    public void Update()
    {
        float distance = Vector2.Distance(_monster.transform.position, transform.position);
        Vector2 monsterDirection = _monster.transform.position - transform.position;
        monsterDirection = monsterDirection.x > 0 ? Vector2.right : Vector2.left;

        bool madnessTime = AbilityController.Instance.Abilities[AbilityType.MADNESS].IsEnabled;
        bool lowHealthPanic = _canLowHealthRun && Health <= HealthThreshold;
        bool proximityPanic = _canProximityPanicRun && distance < PanicDistance;
        bool shouldAttack = _canAttack;
        float rescheduleTime = 5f;

        if (madnessTime)
        {
            lowHealthPanic = lowHealthPanic || !_canAttack;
            proximityPanic = false;
            shouldAttack = !lowHealthPanic;
            rescheduleTime /= 2.0f;
        }

        bool changedDirection = false;
        if ((madnessTime || lowHealthPanic) && UnityEngine.Random.Range(0, 5) >= 3)
        {
            if (Time.time - _lastProximityPanic >= rescheduleTime)
            {
                _lastProximityPanic = Time.time;
                _panicDirection = -_panicDirection;
                changedDirection = true;
            }
        }

        if (!changedDirection && proximityPanic)
        {
            _panicDirection = monsterDirection;
        }

        if (lowHealthPanic || proximityPanic || madnessTime)
        {
            if (_panicDirection == Vector2.right)
            {
                _movement.MoveLeft();
            }
            else
            {
                _movement.MoveRight();
            }
        }

        if (lowHealthPanic || proximityPanic)
        {
            if (_canJump && Time.time - _lastJump >= JUMP_INTERVAL)
            {
                if (UnityEngine.Random.Range(0, 100) >= 80)
                {
                    _movement.Jump();
                    _lastJump = Time.time;
                }
            }
        }
        else if (_canStrike)
        {
            if (StrikeDirection == Vector2.right)
            {
                _movement.MoveRight();
            }
            else
            {
                _movement.MoveLeft();
            }
        }
        else if (shouldAttack)
        {
            Vector2 bulletDirection = monsterDirection;

            // LOOK AT MEE!
            if (!madnessTime)
            {
                if (monsterDirection == Vector2.left)
                {
                    _movement.MoveLeft();
                }
                else
                {
                    _movement.MoveRight();
                }
            }
            else
            {
                bulletDirection = -_panicDirection;
            }

            _movement.Stop();

            if (distance < AttackDistance || madnessTime)
            {
                _monsterFx.setState(MonsterFX.States.ATTACKING);

                if (Time.time - _lastAttack >= (1f / AttackRate))
                {
                    _lastAttack = Time.time;

                    bool isNew;
                    GameObject shot = BulletsPool.Instance.Pop(out isNew);

                    if (isNew)
                    {
                        ThrownDamager.Setup(this, shot, 0.5f, UnityEngine.Random.Range(45, 70), bulletDirection);
                    }

                    shot.transform.position = _bulletPosition.position;

                    if (bulletDirection == Vector2.left)
                    {
                        MobMovement movement = shot.GetComponent<MobMovement>();
                        movement.InitialForce = -movement.InitialForce;
                    }
                }
            }
            else
            {
                _monsterFx.setState(MonsterFX.States.IDLE);
            }
        }
        else
        {
            _movement.Stop();
        }
    }

    void OnDestroy()
    {
        Transform onDestroy = transform.parent.Find("OnDestroy");
        if (onDestroy)
        {
            onDestroy.gameObject.SetActive(true);
            onDestroy.GetComponents<MonoBehaviour>().ToList().ForEach(c => {
                c.enabled = true;
            });
            onDestroy.transform.parent = null;
            Destroy(onDestroy.gameObject, 10f);            
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_canStrike)
        {
            Entity entity = other.gameObject.GetComponent<Entity>();
            if (entity)
            {
                HandleStrikeTo(entity.gameObject);
            }
            else if (other.transform.parent && other.transform.parent.gameObject)
            {
                // Protective shield
                entity = other.transform.parent.gameObject.GetComponentInChildren<Entity>();
                if (entity)
                {
                    HandleStrikeTo(entity.gameObject);
                }
            }
        }
    }

    void HandleStrikeTo(GameObject gameObject)
    {
        Transform onDestroy = transform.parent.Find("OnDestroy");
        if (onDestroy)
        {
            GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Abilities/FireExplosion"));
            explosion.transform.parent = onDestroy.transform;
            explosion.transform.localPosition = new Vector3(0, 0, -0.1f);
            explosion.transform.localScale = onDestroy.localScale;

            _movement.Stop();
            _canStrike = false;
        }

        Destroy(transform.parent.gameObject, 0.5f);
    }
}
