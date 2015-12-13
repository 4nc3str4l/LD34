using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireExplosion : MonoBehaviour
{
    private List<Entity> _toDestroy = new List<Entity>();
    private Collider2D _ownCollider;

    void Start()
    {
        _ownCollider = GetComponent<Collider2D>();
        Destroy(gameObject, 0.52f);
	}

    void OnDestroy()
    {
        _toDestroy.ForEach(entity =>
        {
            entity.DoDamage(100);
            entity.DestroyCallback.Remove(NotifyDeath);
        });
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.gameObject.GetComponentInChildren<Entity>();
        if (entity)
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
