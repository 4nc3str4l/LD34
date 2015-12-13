using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private Dictionary<AbilityType, List<GameObject>> _prefabs = new Dictionary<AbilityType, List<GameObject>>();
    private List<IAbility> _abilities = new List<IAbility>();

    public List<IAbility> Abilities { get { return _abilities; } }
    public List<GameObject> Prefabs(AbilityType type) { return _prefabs[type]; }

    public IAbility BoundAtLeft;
    public IAbility BoundAtRight;

    public void Start()
    {
        Physics2D.IgnoreLayerCollision(Constants.Layers.ABILITIES, Constants.Layers.ABILITIES);
        Physics2D.IgnoreLayerCollision(Constants.Layers.ABILITIES, Constants.Layers.UNIT);
        Physics2D.IgnoreLayerCollision(Constants.Layers.UNIT, Constants.Layers.UNIT);

        LoadPrefabs();
        LoadAbilities();

        BoundAtLeft = _abilities[0];
        BoundAtRight = _abilities[1];
    }

    private void LoadPrefabs()
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Abilities");

        foreach (GameObject gameObject in gameObjects)
        {
            PrefabPlaceholder placeholder = gameObject.GetComponent<PrefabPlaceholder>();
            if (placeholder)
            {
                if (!_prefabs.ContainsKey(placeholder.Type))
                {
                    _prefabs.Add(placeholder.Type, new List<GameObject>());
                }

                _prefabs[placeholder.Type].Add(gameObject);
            }
        }
    }

    private void LoadAbilities()
    {
        Type type = typeof(IAbility);
        IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().
            Where(t => type.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        // Load one by one
        foreach (Type abilityType in types)
        {
            if (!abilityType.IsInterface && !abilityType.IsAbstract)
            {
                IAbility ability = (IAbility)Activator.CreateInstance(abilityType, new object[] { this, gameObject });
                _abilities.Add(ability);
            }
        }
    }

    public void Update()
    {
        foreach (IAbility ability in _abilities)
        {
            ability.Update();
        }

        HandleUserInput();
    }

    private void HandleUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BoundAtLeft.TryFire();
        }

        if (Input.GetMouseButtonDown(1))
        {
            BoundAtRight.TryFire();
        }
    }
}
