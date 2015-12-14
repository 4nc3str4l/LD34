using UnityEngine;

public class MonsterFX : MonoBehaviour {

    const float BASE_INTENSITY = 2.5f;
    const float AURA_BLINK_TIME = 10f;

    public static MonsterFX instance;
    static Vector3 eyelightBasePosition = new Vector3(0, 0.35f, 2.49f);

    private GameObject aura;
    private Light eyeLight;
    private GameObject eyeLightObject;
    private Animator animator;

    public enum States { IDLE, WALKING, ATTACKING }
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
        animator = transform.GetComponent<Animator>();

        if (transform.parent)
        {
            Transform auraTransform = transform.parent.Find("Aura");
            if (auraTransform)
            {
                aura = auraTransform.gameObject;
                aura.SetActive(false);
            }
        }
    }


	void Update ()
    {
        if (eyeLight)
        {
            eyeLight.intensity = BASE_INTENSITY + Random.Range(0f, 1f);
            //eyeLight.transform.localPosition = eyelightBasePosition + new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0f);
        }
    }

    public void setState(States state)
    {
        animator.SetInteger("STATE", (int)state);
    }

    public void toggleAura()
    {
        aura.SetActive(!aura.activeSelf);
    }

    public void enableAura()
    {
        aura.SetActive(true);
    }

    public void disableAura()
    {
        aura.SetActive(false);
    }
}
