using UnityEngine;

public class Aura : MonoBehaviour {

    AudioSource _audioSource;
    bool firstPlay = true;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
	
	void OnEnable()
    {
        if(firstPlay)
        {
            firstPlay = false;
        }
        else
        {
            _audioSource.PlayOneShot(_audioSource.clip);
        }
    }
}
