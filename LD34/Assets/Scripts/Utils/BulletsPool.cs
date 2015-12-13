using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BulletsPool
{
    private static BulletsPool _instance = null;
    public static BulletsPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BulletsPool();
            }

            return _instance;
        }
    }

    private GameObject _original;
    private List<GameObject> _bullets = new List<GameObject>();

    private BulletsPool()
    {
        _original = Resources.Load<GameObject>("Prefabs/Abilities/Shot");
    }

    public void Destroy()
    {
        foreach (GameObject gameObject in _bullets)
        {
            GameObject.Destroy(gameObject);
        }

        GameObject.Destroy(_original);
        _instance = null;
    }

    public GameObject Pop(out bool isNew)
    {
        GameObject shot;
        if (_bullets.Count > 0)
        {
            shot = _bullets[0];
            _bullets.RemoveAt(0);
            isNew = false;
        }
        else
        {
            shot = (GameObject)GameObject.Instantiate(_original);
            isNew = true;
        }

        shot.SetActive(true);
        return shot;
    }

    public void Push(GameObject shot)
    {
        shot.SetActive(false);
        _bullets.Add(shot);
    }
}

