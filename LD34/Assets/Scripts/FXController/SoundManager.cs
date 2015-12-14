using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    public List<AudioClip> clips;

	void Start () {
        DontDestroyOnLoad(this);
	}
	
	void Update () {
	
	}
}
