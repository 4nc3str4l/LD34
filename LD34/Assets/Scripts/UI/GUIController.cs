using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

    public static GUIController instance;
    private Text _numDeadsText;

    void Awake()
    {
        instance = this;
        _numDeadsText = transform.Find("DeadCounter").GetComponent<Text>();
    }

    void Start()
    {

    }

	void Update () {
	
	}

    public void updateDeadCounter()
    {
        _numDeadsText.text = GameController.instance.numDeads.ToString();
    }

}
