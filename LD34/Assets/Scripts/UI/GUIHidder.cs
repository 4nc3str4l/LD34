using UnityEngine;
using System.Collections.Generic;

public class GUIHidder : MonoBehaviour
{

    public List<GameObject> uiElements;

    void Start()
    {
        foreach(GameObject go in uiElements)
        {
            go.SetActive(false);
        }
    }
}
