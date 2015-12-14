using UnityEngine;

public class Rotating : MonoBehaviour {

    const float BLADE_ROTATION_SPEED = 1f;
	
	void Update () {
        transform.RotateAroundLocal(Vector3.forward, BLADE_ROTATION_SPEED);
	}
}
