using UnityEngine;
using UnityEngine.UI;

public class BrutalityBar : MonoBehaviour {

    Image barOverlay;

    void Awake()
    {
        barOverlay = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        barOverlay.fillAmount = (float)GameController.instance.numDeads / GameController.NUM_DEADS_TO_WIN;
	}
}
