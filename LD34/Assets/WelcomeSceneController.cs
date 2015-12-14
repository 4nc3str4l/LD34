using UnityEngine;
using System.Collections;

public class WelcomeSceneController : MonoBehaviour {

    public void onPlay()
    {
        Application.LoadLevel("Loading");
    }
}
