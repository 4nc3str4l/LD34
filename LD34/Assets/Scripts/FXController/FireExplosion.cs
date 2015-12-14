using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireExplosion : MonoBehaviour
{
    private List<Entity> _toDestroy = new List<Entity>();
    private Collider2D _ownCollider;
    private Animator _animator;
    private int _nextState = 0;

    private bool _shouldExplode = false;
    private float _startTime = 0;

    public bool IgnoreMonster = true;

    void Start()
    {
        _ownCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
	}

    public void ToggleAnimation()
    {
        _animator.SetInteger("STATE", _nextState);
        _nextState ^= 1;
    }

    void OnEnable()
    {
        _shouldExplode = true;
        _startTime = Time.time;
    }

    void Update()
    {
        if (_shouldExplode && Time.time - _startTime >= 0.3f)
        {
            _toDestroy.ForEach(entity =>
            {
                entity.DoDamage(100);
                entity.DestroyCallback.Remove(NotifyDeath);
            });

            _shouldExplode = false;

            if (IgnoreMonster)
            {
                ExplosionsPool.Instance.Push(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.gameObject.GetComponentInChildren<Entity>();
        if (entity && (!IgnoreMonster || entity != GameController.Instance.Monster))
        {
            _toDestroy.Add(entity);
            entity.DestroyCallback.Add(NotifyDeath);
        }
        else
        {
            Physics2D.IgnoreCollision(_ownCollider, other);
        }
    }

    void NotifyDeath(Entity entity)
    {
        _toDestroy.Remove(entity);
    }
}
