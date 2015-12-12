using UnityEngine;
using UnityEngine.UI;

public class SkillInspector : MonoBehaviour {

    Text title;
    Text description;
    Image image;

	void Start () {
        title = transform.Find("Title").GetComponent<Text>();
        description = transform.Find("Description").GetComponent<Text>();
        image = transform.Find("Image").GetComponent<Image>();
    }
	
    public void setInspectedItem(string titl, string desc, Sprite img)
    {
        title.text = titl;
        description.text = desc;
        image.sprite = img;
    }
}
