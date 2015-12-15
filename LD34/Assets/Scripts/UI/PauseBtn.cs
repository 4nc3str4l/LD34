using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PauseBtn : MonoBehaviour
{
    public void Continue()
    {
        PauseMenu.Instance.ResumeGame();
        GUIController.instance.togglePauseMenu();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = GameObject.Find("Slider").GetComponent<Slider>().value;
    }

    public void ShowPause()
    {
        PauseMenu.Instance.PauseGame();
        GUIController.instance.togglePauseMenu();
    }
}

