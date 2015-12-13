using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Flags]
enum MobFlags
{
    PROXIMITY_PANIC,
    LOW_HEALTH_PANIC,
    ATTACK
}

class Mob : MonoBehaviour
{
    [SerializeField] [EnumFlagsAttribute]
    public MobFlags Flags = MobFlags.PROXIMITY_PANIC | MobFlags.LOW_HEALTH_PANIC;
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
    }

    public void Update()
    {
        float distance = Vector2.Distance(_monster.transform.position, transform.position);

        bool lowHealthPanic = (Flags & MobFlags.LOW_HEALTH_PANIC) == MobFlags.LOW_HEALTH_PANIC && Health <= HealthThreshold;
        bool proximityPanic = (Flags & MobFlags.PROXIMITY_PANIC) == MobFlags.PROXIMITY_PANIC && distance < PanicDistance;

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

            if (Time.time - _lastJump >= JUMP_INTERVAL)
            {
                if (UnityEngine.Random.Range(0, 100) >= 80)
                {
                    _movement.Jump();
                    _lastJump = Time.time;
                }
            }
        }
        else
        {
            _movement.Stop();
        }
    }
}
