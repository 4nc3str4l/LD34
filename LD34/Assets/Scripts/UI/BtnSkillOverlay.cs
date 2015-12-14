using UnityEngine;
using UnityEngine.UI;

public class BtnSkillOverlay : MonoBehaviour {

    public enum CONTROLLING_BUTTON { LEFT_BUTTON , RIGHT_BUTTON }
    public CONTROLLING_BUTTON controllingButton;
    public Image overlay;
    float _percentatge;

	void Update () {
        if (controllingButton == CONTROLLING_BUTTON.LEFT_BUTTON)
        {
            _percentatge = AbilityController.Instance.BoundAtLeft.RemainingCooldown / AbilityController.Instance.BoundAtLeft.CooldownTime;
        }
        else
        {
            _percentatge = AbilityController.Instance.BoundAtRight.RemainingCooldown / AbilityController.Instance.BoundAtRight.CooldownTime;
        }
        overlay.fillAmount = _percentatge;
    }
}
