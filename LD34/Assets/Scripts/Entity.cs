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

    public void DoDamage(float damage)
    {
        _health = Mathf.Max(0, _health - damage);
        OnDamaged();

        if (_health <= 0)
        {
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

    protected virtual void OnDamaged() { }
    protected virtual void OnRestored() { }
}
