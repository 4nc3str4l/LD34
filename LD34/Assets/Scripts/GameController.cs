using UnityEngine;

public class GameController : MonoBehaviour {

    int _numDeads = 0;
    public int numDeads {get { return _numDeads; }}

    public static GameController instance;


    void Awake()
    {
        instance = this;
    }

    public void addDead()
    {
        _numDeads++;
        GUIController.instance.updateDeadCounter();
        MonsterFX.instance.blinkAura();
    }
}
