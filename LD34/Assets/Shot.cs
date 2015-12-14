using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {

    // Use this for initialization
    public AudioSource _source;

    void OnEnable()
    {
        _source.PlayOneShot(_source.clip, 0.3f);
    }
}
