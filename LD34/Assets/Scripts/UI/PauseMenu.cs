using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public enum PauseState { NOT_PAUSED, PAUSING, PAUSED, EXITING }
    private PauseState _state;
    public PauseState State { get { return _state; } }

    public static PauseMenu Instance;
        
    public void Awake()
    {
        Instance = this;
    }   

    private void Update()
    {
        switch (_state)
        {
            case PauseState.NOT_PAUSED:
                break;

            case PauseState.PAUSING:
                if (AbilityController.Instance.Abilities[AbilityType.PROTECTION_FIELD].IsEnabled)
                {
                    AbilityController.Instance.Abilities[AbilityType.PROTECTION_FIELD].OnEnd();
                }
                GameController.Instance.Monster.GetComponentInChildren<MonsterFX>().enableAura();
                _state = PauseState.PAUSED;
                break;

            case PauseState.PAUSED:
                break;

            case PauseState.EXITING:
                GameController.Instance.Monster.GetComponentInChildren<MonsterFX>().disableAura();
                _state = PauseState.NOT_PAUSED;
                break;

            default:
                break;
        }
    }

    public void PauseGame()
    {
        _state = PauseState.PAUSING;
    }

    public void ResumeGame()
    {
        _state = PauseState.EXITING;
    }

    public void ToggleState()
    {
        if (_state == PauseState.NOT_PAUSED)
        {
            _state = PauseState.PAUSING;
        }
        else
        {
            _state = PauseState.EXITING;
        }
    }
}
