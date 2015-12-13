using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BtnSkill : MonoBehaviour {

    public string title;
    public string description;
    public Sprite image;

    SkillInspector skillInspector;
    SkillInspector skillInspectorRight;

    public AbilityType ability;  

    void Start()
    {
        skillInspector = GameObject.Find("SkillInspector").GetComponent<SkillInspector>();
        skillInspectorRight = GameObject.Find("SkillInspectorRight").GetComponent<SkillInspector>();
    }

	public void onMouseHover()
    {
        skillInspector.gameObject.SetActive(true);
        skillInspector.setInspectedItem(title, description, image);
    }

    public void onMouseOut()
    {
        skillInspector.gameObject.SetActive(false);
    }

    public void onMouseHoverRight()
    {
        skillInspectorRight.gameObject.SetActive(true);
        skillInspectorRight.setInspectedItem(title, description, image);
    }

    public void onMouseOutRight()
    {
        skillInspectorRight.gameObject.SetActive(false);
    }

    public void setNewInfo(BtnSkill skill)
    {
        image = skill.image;
        description = skill.description;
        title = skill.title;
        ability = skill.ability;
        GetComponent<Image>().sprite = image;
    }
}
