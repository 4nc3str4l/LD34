using UnityEngine;

public class MonsterFX : MonoBehaviour {

    const float BASE_INTENSITY = 2.5f;
    const float AURA_BLINK_TIME = 10f;

    public static MonsterFX instance;
    static Vector3 eyelightBasePosition = new Vector3(0, 0.35f, 2.49f);

    Light eyeLight, aura;
    GameObject eyeLightObject;
    Animator animator;

    public enum States { IDLE, WALKING}
    public int state = 0;
    float _stopAura;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        eyeLightObject = GameObject.Find("EyeLight");
        eyeLight = eyeLightObject.GetComponent<Light>();
        animator = GetComponent<Animator>();
        aura = GameObject.Find("Aura").GetComponent<Light>();
        aura.enabled = false;
    }	


	void Update ()
    {
        if (eyeLight)
        {
            eyeLight.intensity = BASE_INTENSITY + Random.Range(0f, 1f);
            //eyeLight.transform.localPosition = eyelightBasePosition + new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0f);

            if (aura.enabled && _stopAura < Time.time)
            {
                aura.enabled = false;
            }
        }
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

    public void blinkAura()
    {
        _stopAura = Time.time + AURA_BLINK_TIME;
        aura.enabled  = true;
    }
}
