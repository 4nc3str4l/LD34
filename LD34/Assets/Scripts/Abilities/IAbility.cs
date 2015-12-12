using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

interface IAbility
{
    string Name { get; }

    bool CanBeUsed { get; }
    float CooldownTime { get; }
    float RemainingCooldown { get; }

    void OnEnable();
    void Update();
    void OnEnded();
}
