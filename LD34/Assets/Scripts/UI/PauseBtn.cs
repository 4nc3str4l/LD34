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
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ShowPause()
    {
        PauseMenu.Instance.PauseGame();
    }
}

