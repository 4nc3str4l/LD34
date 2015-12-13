using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Ability : IAbility
{
    public abstract string Name { get; }
    public abstract AbilityType Type { get; }
    public abstract float CooldownTime { get; }
    public abstract float DestroyTime { get; }
    public abstract float DamageInterval { get; }
    public abstract float DamagePerHit { get; }

    public bool IsEnabled { get { return _enabled; } }
    public GameObject Owner { get { return _owner; } }
    public virtual bool CanBeUsed { get { return RemainingCooldown == 0; } }
    public float RemainingCooldown { get { return _remainingCooldown; } }

    protected bool _enabled = false;
    protected float _remainingCooldown = 0;
    protected float _elapsedTime = 0;
    protected GameObject _owner;
    protected List<GameObject> _prefabs;

    public Ability(AbilityController controller, GameObject owner)
    {
        _owner = owner;
        _prefabs = controller.Prefabs(Type);
    }

    public bool TryFire()
    {
        if (CanBeUsed)
        {
            OnStart();
            return true;
        }

        return false;
    }

    public virtual void OnEnd()
    {
        _enabled = false;
    }

    public virtual void OnStart()
    {
        Assert.IsTrue(CanBeUsed);

        _remainingCooldown = CooldownTime;
        _elapsedTime = 0;
        _enabled = true;
    }

    public virtual void Update()
    {
        if (_enabled)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= DestroyTime)
            {
                OnEnd();
            }
        }

        if (_remainingCooldown > 0)
        {
            _remainingCooldown -= Time.deltaTime;
        }
        else if (_remainingCooldown < 0)
        {
            _remainingCooldown = 0;
        }
    }

    protected void setupGameobject<T>(GameObject gameObject) where T : AbilityDamager
    {
        gameObject.AddComponent<T>();
        gameObject.GetComponent<T>().Owner = this;
    }

    protected void setupGameobject(GameObject gameObject)
    {
        setupGameobject<AbilityDamager>(gameObject);
    }
}

public class AbilityDamager : MonoBehaviour
{
    public IAbility Owner;
    private float _lastHit;
    private Collider2D _ownCollider;

    void Start()
    {
        _ownCollider = GetComponent<Collider2D>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        handleTrigger(other);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        handleTrigger(other);
    }

    private void handleTrigger(Collider2D other)
    {
        Mob mob = other.gameObject.GetComponentInChildren<Mob>();
        if (mob)
        {
            DoDamage(mob);
        }
        else
        {
            Physics2D.IgnoreCollision(_ownCollider, other);
        }
    }

    protected virtual void DoDamage(Mob mob)
    {
        if (Time.time - _lastHit >= Owner.DamageInterval)
        {
            _lastHit = Time.time;
            mob.DoDamage(Owner.DamagePerHit);
        }
    }
}

public sealed class ThrownDamager : MonoBehaviour
{
    public Mob Owner;
    public float Damage;
    public float MadnessDamage;
    public Vector2 MoveDirection = Vector2.zero;
    private float _lastHit;
    private float _spawnTime;
    private MobMovement Movement;
    private Collider2D _ownCollider;

    public static void Setup(Mob owner, GameObject gameObject, float damage, float madnessDamage, Vector2 moveDirection)
    {
        gameObject.AddComponent<ThrownDamager>();
        ThrownDamager damager = gameObject.GetComponent<ThrownDamager>();
        damager.Owner = owner;
        damager.Damage = damage;
        damager.MadnessDamage = madnessDamage;
        damager.MoveDirection = moveDirection;
        damager.Movement = gameObject.GetComponent<MobMovement>();
    }

    public static void Setup(Mob owner, GameObject gameObject, float damage, float madnessDamage)
    {
        Setup(owner, gameObject, damage, madnessDamage, Vector2.zero);
    }

    void Start()
    {
        _ownCollider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        _spawnTime = Time.time;
    }

    void Update()
    {
        if (MoveDirection == Vector2.right)
        {
            Movement.MoveRight();
        }
        else if (MoveDirection == Vector2.left)
        {
            Movement.MoveLeft();
        }

        if (Time.time - _spawnTime > 5f)
        {
            BulletsPool.Instance.Push(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        testTrigger(other);
    }

    private void testTrigger(Collider2D other)
    {
        Debug.Log(other.gameObject.name + " " + _ownCollider);
        if (other.transform.parent && other.transform.parent.gameObject)
        {
            Debug.Log("\t" + other.transform.parent.gameObject.name);
        }
        Mob mob = other.gameObject.GetComponentInChildren<Mob>();
        if (mob)
        {
            bool madnessTime = AbilityController.Instance.Abilities[AbilityType.MADNESS].IsEnabled;
            if (madnessTime && mob != Owner)
            {
                mob.DoDamage(MadnessDamage);
                BulletsPool.Instance.Push(gameObject);
            }
            else
            {
                Debug.Log("\t1: Ignoring");
                Physics2D.IgnoreCollision(_ownCollider, other);
            }
        }
        else
        {
            MonsterController monster = other.gameObject.GetComponentInChildren<MonsterController>();
            if (monster)
            {
                monster.DoDamage(Damage);
                BulletsPool.Instance.Push(gameObject);
            }
            else
            {
                Debug.Log("\t2: Ignoring");
                Physics2D.IgnoreCollision(_ownCollider, other);
            }
        }
    }
}

