using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum AbilityType
{
    SOUL_FIRE,
    SOUL_FILE_TARGETED
}

public interface IAbility
{
    string Name { get; }
    AbilityType Type { get; }
    GameObject Owner { get; }

    bool CanBeUsed { get; }
    float CooldownTime { get; }
    float RemainingCooldown { get; }

    void OnStart();
    void Update();
    void OnEnd();
}
