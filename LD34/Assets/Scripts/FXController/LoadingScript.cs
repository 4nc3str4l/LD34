using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour {


    public const float LETTER_SPAWN_RATE = 1f;
    public Text text;
    public string loadingText = "Loading...";
    public string actualText = "Loading";
    public float nextLetterSpawn = 0f;
    int i = 7;

    void Start()
    {
        Application.LoadLevel("C_GameScene");
    }
	
	// Update is called once per frame
	void Update () {
	    if(nextLetterSpawn < Time.time)
        {
            text.text = loadingText.Substring(0, i);
            i++;
            nextLetterSpawn = Time.time + LETTER_SPAWN_RATE;
            if(i > loadingText.Length)
            {
                i = 7;
            }
        }
	}
}
