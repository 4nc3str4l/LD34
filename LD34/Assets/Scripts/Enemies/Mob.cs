using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Mob : MonoBehaviour
{
    public float Health { get { return _health; } }
    private float _health = 100;

    public void DoDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
