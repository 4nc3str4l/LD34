using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

class AbilityController : MonoBehaviour
{
    private List<IAbility> _abilities = new List<IAbility>();
    public List<IAbility> Abilities { get { return _abilities; } }

    void Start()
    {
        Type type = typeof(IAbility);
        IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().
            Where(t => type.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
        
        // Load one by one
        foreach (Type abilityType in types)
        {
            if (!abilityType.IsInterface && !abilityType.IsAbstract)
            {
                IAbility ability = (IAbility)Activator.CreateInstance(abilityType, new object[] { gameObject });
                _abilities.Add(ability);
            }
        }
    }

    void Update()
    {
        foreach (IAbility ability in _abilities)
        {
            ability.Update();
        }
    }
}
