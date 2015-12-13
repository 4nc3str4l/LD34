using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Flags]
enum MobFlags
{
    CAN_JUMP            = 1,
    PROXIMITY_PANIC     = 2,
    LOW_HEALTH_PANIC    = 4,
    STRIKE_EVERYONE     = 8,
    ATTACK              = 16
}

class Mob : MonoBehaviour
{
    [SerializeField] [EnumFlagsAttribute]
    public MobFlags Flags = MobFlags.CAN_JUMP | MobFlags.PROXIMITY_PANIC | MobFlags.LOW_HEALTH_PANIC;
    public float PanicDistance = 5f;
    public float HealthThreshold = 10f;

    public float Health { get { return _health; } }
    private float _health = 100;

    private const float JUMP_INTERVAL = 5f;

    private GameObject _monster;
    private MobMovement _movement;
    private Vector2 _panicDirection;
    private float _lastJump;
    private float _lastProximityPanic;

    private bool _canJump;
    private bool _canProximityPanicRun;
    private bool _canLowHealthRun;
    private bool _canStrike;

    public void DoDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    public void Start()
    {
        _monster = GameObject.Find("MonsterContainer");
        _movement = transform.parent.GetComponent<MobMovement>();

        _canJump = (Flags & MobFlags.CAN_JUMP) == MobFlags.CAN_JUMP;
        _canLowHealthRun = (Flags & MobFlags.LOW_HEALTH_PANIC) == MobFlags.LOW_HEALTH_PANIC;
        _canProximityPanicRun = (Flags & MobFlags.PROXIMITY_PANIC) == MobFlags.PROXIMITY_PANIC;
        _canStrike = (Flags & MobFlags.STRIKE_EVERYONE) == MobFlags.STRIKE_EVERYONE;
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
            _movement.MoveRight();
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
            onDestroy.GetComponents<MonoBehaviour>().ToList().ForEach(c => c.enabled = true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_canStrike)
        {
            Mob mob = other.gameObject.GetComponentInChildren<Mob>();
            if (mob)
            {
                HandleStrikeTo(mob.gameObject);
            }
            else
            {
                // TODO: Replace for real controller
                AbilityController controller = other.gameObject.GetComponentInChildren<AbilityController>();
                if (controller)
                {
                    // TODO: Apply damage to monster
                    HandleStrikeTo(controller.gameObject);
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
