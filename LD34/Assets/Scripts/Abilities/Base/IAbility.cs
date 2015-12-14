using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum AbilityType
{
    SOUL_FIRE,
    SOUL_FIRE_TARGETED,
    DEATH_STARE,
    RADIOACTIVE_SPARK,
    PROTECTION_FIELD,
    MADNESS,
    HUMAN_GRINDER,
    RADIOACTIVE_PARTY
}

public interface IAbility
{
    string Name { get; }
    AbilityType Type { get; }
    GameObject Owner { get; }
    bool IsEnabled { get; }

    bool CanBeUsed { get; }
    float CooldownTime { get; }
    float RemainingCooldown { get; }
    float DestroyTime { get; }
    float DamageInterval { get; }
    float DamagePerHit { get; }

    bool TryFire();
    void OnStart();
    void Update();
    void OnEnd();
}
