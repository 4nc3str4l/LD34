using System;
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
    private Vector2 _panicDirection;
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
        _movement = transform.parent.GetComponent<MobMovement>();

        _canJump = (Flags & MobFlags.CAN_JUMP) == MobFlags.CAN_JUMP;
        _canLowHealthRun = (Flags & MobFlags.LOW_HEALTH_PANIC) == MobFlags.LOW_HEALTH_PANIC;
        _canProximityPanicRun = (Flags & MobFlags.PROXIMITY_PANIC) == MobFlags.PROXIMITY_PANIC;
        _canStrike = (Flags & MobFlags.STRIKE_EVERYONE) == MobFlags.STRIKE_EVERYONE;
        _canAttack = (Flags & MobFlags.ATTACK) == MobFlags.ATTACK;
    }

    public void Update()
    {
        float distance = Vector2.Distance(_monster.transform.position, transform.position);

        bool lowHealthPanic = _canLowHealthRun && Health <= HealthThreshold;
        bool proximityPanic = _canProximityPanicRun && distance < PanicDistance;

        if (lowHealthPanic || proximityPanic)
        {
            if (lowHealthPanic && UnityEngine.Random.Range(0, 5) >= 3)
            {
                if (Time.time - _lastProximityPanic >= 5f)
                {
                    _lastProximityPanic = Time.time;
                    _panicDirection = -_panicDirection;
                }
            }
            else if (proximityPanic)
            {
                _panicDirection = _monster.transform.position - transform.position;
            }

            if (_panicDirection.x > 0)
            {
                _movement.MoveLeft();
            }
            else
            {
                _movement.MoveRight();
            }

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
        else if (_canAttack)
        {
            _movement.Stop();

            if (distance < AttackDistance)
            {
                if (Time.time - _lastAttack > (1f / AttackRate))
                {
                    Debug.Log("Doing Damage");
                    _lastAttack = Time.time;
                    _monster.DoDamage(10);
                }
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
            Entity entity = other.gameObject.GetComponentInChildren<Entity>();
            if (entity)
            {
                HandleStrikeTo(entity.gameObject);
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
