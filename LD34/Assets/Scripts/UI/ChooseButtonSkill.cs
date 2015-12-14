using UnityEngine;

public class ChooseButtonSkill : MonoBehaviour {

    Vector3 targetPostion = Vector3.zero;
    public bool choosed = false;
    public AbilityType ability;
    AudioSource _audioSource;
    public AudioClip merging, fire, fire_distance, deathStare, madness, humanGrinder, shield, doNotPress, spark;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update () {
        if (targetPostion != Vector3.zero)
        {
            if(transform.localPosition != targetPostion)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPostion, 10f);
            }
            else
            {
                targetPostion = Vector3.zero;
            }
        }
    }

    public void moveToPosition(Vector3 position)
    {
        targetPostion = position;
        _audioSource.PlayOneShot(merging, 0.5f);
    }

    public void AssociateAbility(AbilityType ab)
    {
        ability = ab;
    }

    public void MouseDown()
    {
        if(GUIController.instance.actualChooseAnimationState == GUIController.ChooseAnimationState.CHOOSING)
        {
            AudioClip c = null;
            switch(ability)
            {
                case AbilityType.DEATH_STARE:
                    c = deathStare;
                    break;
                case AbilityType.HUMAN_GRINDER:
                    c = humanGrinder;
                    break;
                case AbilityType.MADNESS:
                    c = madness;
                    break;
                case AbilityType.PROTECTION_FIELD:
                    c = shield;
                    break;
                case AbilityType.RADIOACTIVE_PARTY:
                    c = doNotPress;
                    break;
                case AbilityType.RADIOACTIVE_SPARK:
                    break;
                case AbilityType.SOUL_FIRE:
                    c = fire;
                    break;
                case AbilityType.SOUL_FIRE_TARGETED:
                    c = fire_distance;
                    break;
                default:
                    break;
            }

            GUIController.instance.chooseSkill(ability, c);
            this.gameObject.SetActive(false);
        }

    }
}
