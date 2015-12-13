using UnityEngine;

public class ChooseButtonSkill : MonoBehaviour {

    Vector3 targetPostion = Vector3.zero;
    Ability ability;

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
    }

    public void AssociateAbility(Ability ab)
    {
        ability = ab;
    }
}
