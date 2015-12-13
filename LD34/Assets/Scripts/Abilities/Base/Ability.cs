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

    public GameObject Owner { get { return _owner; } }
    public virtual bool CanBeUsed { get { return RemainingCooldown == 0; } }
    public float RemainingCooldown { get { return _remainingCooldown; } }

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

    public virtual void OnEnd() { }

    public virtual void OnStart()
    {
        Assert.IsTrue(CanBeUsed);
        _remainingCooldown = CooldownTime;
        _elapsedTime = 0;
    }

    public virtual void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= DestroyTime)
        {
            OnEnd();
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

    protected void setupGameobject(GameObject gameObject)
    {
        gameObject.AddComponent<AbilityDamager>();
        gameObject.GetComponent<AbilityDamager>().Owner = this;
    }
}

class AbilityDamager : MonoBehaviour
{
    public IAbility Owner;
    private float _lastHit;

    public void OnTriggerEnter2D(Collider2D other)
    {
        Mob mob = other.gameObject.GetComponentInChildren<Mob>();
        if (mob)
        {
            DoDamage(mob);
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        Mob mob = other.gameObject.GetComponentInChildren<Mob>();
        if (mob)
        {
            DoDamage(mob);
        }
    }

    private void DoDamage(Mob mob)
    {
        if (Time.time - _lastHit >= Owner.DamageInterval)
        {
            _lastHit = Time.time;
            mob.DoDamage(Owner.DamagePerHit);
        }
    }
}
