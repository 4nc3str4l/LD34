using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    private Dictionary<AbilityType, List<GameObject>> _prefabs = new Dictionary<AbilityType, List<GameObject>>();
    private Dictionary<AbilityType, IAbility> _abilities = new Dictionary<AbilityType, IAbility>();

    public Dictionary<AbilityType, IAbility> Abilities { get { return _abilities; } }

    public IAbility BoundAtLeft;
    public IAbility BoundAtRight;

    public static AbilityController Instance;

    public void Start()
    {
        Physics2D.IgnoreLayerCollision(Constants.Layers.ABILITIES, Constants.Layers.ABILITIES);
        Physics2D.IgnoreLayerCollision(Constants.Layers.ABILITIES, Constants.Layers.UNIT);
        Physics2D.IgnoreLayerCollision(Constants.Layers.UNIT, Constants.Layers.UNIT);

        Instance = this;

        LoadPrefabs();
        LoadAbilities();

        BoundAtLeft = _abilities[AbilityType.RADIOACTIVE_PARTY];
        BoundAtRight = _abilities[AbilityType.MADNESS];
    }

    public List<GameObject> Prefabs(AbilityType type)
    {
        if (_prefabs.ContainsKey(type))
        {
            return _prefabs[type];
        }

        return new List<GameObject>();
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
                _abilities.Add(ability.Type, ability);
            }
        }
    }

    public void Update()
    {
        foreach (IAbility ability in _abilities.Values)
        {
            ability.Update();
        }

        HandleUserInput();
    }

    private void HandleUserInput()
    {
        if (GUIController.instance.actualChooseAnimationState != GUIController.ChooseAnimationState.STOPPED)
        {
            return;
        }
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
