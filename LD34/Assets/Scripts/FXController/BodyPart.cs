using UnityEngine;
using System.Collections;

public class BodyPart : MonoBehaviour {

    bool _exploded = false;

    void Start () {

	}
	

	void Update () {
        if (this.transform.parent.gameObject.active && !_exploded)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-10f, 10f) * 1000, UnityEngine.Random.Range(-10f, 10f) * 1000));
            Destroy(gameObject, 0.4f);
            _exploded = true;
        }
	}
}
