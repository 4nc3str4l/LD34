using UnityEngine;

public class MonsterFX : MonoBehaviour {
    
    const float BASE_INTENSITY = 2.5f;
    static Vector3 eyelightBasePosition = new Vector3(0, 0.35f, 2.49f);

    Light eyeLight;
    GameObject eyeLightObject;
    Animator animator;

    public enum States { IDLE, WALKING}
    public int state = 0;

    void Start()
    {
        eyeLightObject = GameObject.Find("EyeLight");
        eyeLight = eyeLightObject.GetComponent<Light>();
        animator = GetComponent<Animator>();
    }	


	void Update ()
    { 
        eyeLight.intensity = BASE_INTENSITY + Random.Range(0f, 1f);
        eyeLight.transform.localPosition = eyelightBasePosition + new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0f);
    }

    public void setState(States state)
    {
        switch (state)
        {

            case States.IDLE:
                animator.SetInteger("STATE", 0);
                break;
            case States.WALKING:
                animator.SetInteger("STATE", 1);
                break;
        } 
    }
}
