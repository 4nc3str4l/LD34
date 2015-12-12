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

    public GameObject Owner { get { return _owner; } }
    public virtual bool CanBeUsed { get { return RemainingCooldown == 0; } }
    public float RemainingCooldown { get { return _remainingCooldown; } }

    protected float _remainingCooldown;
    protected GameObject _owner;
    protected List<GameObject> _prefabs;

    public Ability(AbilityController controller, GameObject owner)
    {
        _owner = owner;
        _prefabs = controller.Prefabs(Type);
    }

    public virtual void OnEnd() { }

    public virtual void OnStart()
    {
        Assert.IsTrue(CanBeUsed);

        if (_remainingCooldown > 0)
        {
            _remainingCooldown = CooldownTime;
        }
    }

    public virtual void Update()
    {
        _remainingCooldown -= Time.deltaTime;
    }
}
