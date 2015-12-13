using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool CanRecoverHealth = false;
    public float RecoverInterval = 1f;
    public float RecoverAmount = 2f;

    public float Health { get { return _health; } }
    private float _health = 100;
    private float _lastRecover = 0;

    public List<Action<Entity>> DestroyCallback = new List<Action<Entity>>();

    public void DoDamage(float damage)
    {
        _health = OnDamaged(Mathf.Max(0, _health - damage));

        if (_health <= 0)
        {
            DestroyCallback.ToList().ForEach(f => f(this));
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    void Update()
    {
        if (Health < 100 && Time.time - _lastRecover > RecoverInterval)
        {
            _lastRecover = Time.time;
            _health = Mathf.Max(100, Health + RecoverAmount);
            OnRestored();
        }
    }

    protected virtual float OnDamaged(float newHealth)
    {
        return newHealth;
    }

    protected virtual void OnRestored() { }
}
