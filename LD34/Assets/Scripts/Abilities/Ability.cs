using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

abstract class Ability : IAbility
{
    public abstract string Name { get; }
    public abstract float CooldownTime { get; }

    public virtual bool CanBeUsed { get { return RemainingCooldown == 0; } }

    private float _remainingCooldown;
    public float RemainingCooldown { get { return _remainingCooldown; } }

    public virtual void OnEnded()
    {
    }

    public virtual void OnEnable()
    {
        Assert.IsTrue(CanBeUsed);

        _remainingCooldown = CooldownTime;
    }

    public virtual void Update()
    {
        _remainingCooldown -= Time.deltaTime;
    }
}
