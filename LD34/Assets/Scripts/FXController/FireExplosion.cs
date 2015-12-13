using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireExplosion : MonoBehaviour
{
    private List<Entity> _toDestroy = new List<Entity>();

	void Start()
    {
	    Destroy(gameObject, 0.52f);
	}

    void OnDestroy()
    {
        _toDestroy.ForEach(entity => entity.DoDamage(100));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.gameObject.GetComponentInChildren<Entity>();
        if (entity)
        {
            _toDestroy.Add(entity);
        }
    }
}
