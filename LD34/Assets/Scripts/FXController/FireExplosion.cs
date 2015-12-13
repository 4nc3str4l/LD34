using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireExplosion : MonoBehaviour
{
    private List<GameObject> _toDestroy = new List<GameObject>();

	void Start()
    {
	    Destroy(gameObject, 0.52f);
	}

    void OnDestroy()
    {
        _toDestroy.ForEach(gob => Destroy(gob));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob)
        {
            _toDestroy.Add(mob.transform.parent.gameObject);
        }
        else
        {
            // TODO: Replace for real controller
            AbilityController controller = other.gameObject.GetComponentInChildren<AbilityController>();
            if (controller)
            {
                // TODO: Apply damage to monster
                _toDestroy.Add(controller.transform.parent.gameObject);
            }
        }
    }
}
