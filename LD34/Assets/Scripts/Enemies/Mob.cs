using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Flags]
enum MobFlags
{
    PANIC,
    ATTACK
}

class Mob : MonoBehaviour
{
    public MobFlags Flags = MobFlags.PANIC;
    public float PanicDistance = 7f;

    public float Health { get { return _health; } }
    private float _health = 100;

    private const float JUMP_INTERVAL = 5f;

    private GameObject _monster;
    private MobMovement _movement;
    private float _lastJump;

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
        if (distance < PanicDistance && (Flags & MobFlags.PANIC) == MobFlags.PANIC)
        {
            Vector2 direction = _monster.transform.position - transform.position;

            if (direction.x > 0)
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
