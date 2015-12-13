using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ExplosionsPool
{
    private static ExplosionsPool _instance = null;
    public static ExplosionsPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ExplosionsPool();
            }

            return _instance;
        }
    }

    private GameObject _original;
    private List<GameObject> _explosions = new List<GameObject>();

    private ExplosionsPool()
    {
        _original = Resources.Load<GameObject>("Prefabs/Abilities/FireExplosion");
    }

    public GameObject Pop()
    {
        GameObject explosion;
        if (_explosions.Count > 0)
        {
            explosion = _explosions[0];
            _explosions.RemoveAt(0);
            explosion.GetComponent<FireExplosion>().ToggleAnimation();
        }
        else
        {
            explosion = GameObject.Instantiate(_original);
        }

        explosion.SetActive(true);
        return explosion;
    }

    public void Push(GameObject explosion)
    {
        explosion.SetActive(false);
        _explosions.Add(explosion);
    }
}

static class ListExtension
{
}
