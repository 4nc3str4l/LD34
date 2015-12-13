using UnityEngine;

public class GameController : MonoBehaviour {

    public const int NUM_DEADS_TO_WIN = 10;

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
    }

    public void OnDestroy()
    {
        BulletsPool.Instance.Destroy();
        ExplosionsPool.Instance.Destroy();
    }
}
