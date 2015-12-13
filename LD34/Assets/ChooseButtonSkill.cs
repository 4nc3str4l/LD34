﻿using UnityEngine;

public class ChooseButtonSkill : MonoBehaviour {

    Vector3 targetPostion = Vector3.zero;
    public bool choosed = false;
    public AbilityType ability;

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

    public void AssociateAbility(AbilityType ab)
    {
        ability = ab;
    }

    public void MouseDown()
    {
        if(GUIController.instance.actualChooseAnimationState == GUIController.ChooseAnimationState.CHOOSING)
        {
            GUIController.instance.chooseSkill(ability);
            this.gameObject.SetActive(false);
        }

    }
}
