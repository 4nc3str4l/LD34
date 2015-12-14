using UnityEngine;

public class GameController : MonoBehaviour {

    public const int NUM_DEADS_TO_WIN = 10;

    int _numDeads = 0;
    public int numDeads {get { return _numDeads; }}

    public static GameController Instance;

    private const float SPAWN_EVERY = 15f;

    public MonsterController Monster;
    private GameObject _originalHuman;
    private GameObject _originalSoldier;
    private float _lastExplored = 0;
    private float _lastSpawned = 0;

    void Awake()
    {
        Instance = this;
        Monster = GameObject.Find("MonsterContainer").GetComponentInChildren<MonsterController>();
        _originalHuman = Resources.Load<GameObject>("Prefabs/Human");
        _originalSoldier = Resources.Load<GameObject>("Prefabs/Soldier");
    }

    void Update()
    {
        float width = Camera.main.orthographicSize * Screen.width / Screen.height;
        _lastExplored = Mathf.Max(_lastExplored, Monster.transform.position.x + width / 2);

        if (_lastExplored + SPAWN_EVERY > _lastSpawned + SPAWN_EVERY / 2)
        {
            _lastSpawned = _lastExplored + SPAWN_EVERY;

            for (int i = 0; i < 5; ++i)
            {
                float y = UnityEngine.Random.Range(0.5f, 15);
                if (UnityEngine.Random.Range(0, 64) >= 32)
                {
                    GameObject.Instantiate(_originalSoldier, new Vector2(_lastSpawned + i, y), Quaternion.identity);
                }
                else
                {
                    GameObject.Instantiate(_originalHuman, new Vector2(_lastSpawned + i, y), Quaternion.identity);
                }
            }
        }
    }

    public void addDead()
    {
        _numDeads++;
        GUIController.instance.updateDeadCounter();

        if (_numDeads % 30 == 0 && GUIController.instance.actualChooseAnimationState == GUIController.ChooseAnimationState.STOPPED)
        {
            GUIController.instance.actualChooseAnimationState = GUIController.ChooseAnimationState.PREPARING;
        }

        if (_numDeads > NUM_DEADS_TO_WIN)
        {
            GUIController.instance.showWinAnimation();
        }
    }

    public void OnDisable()
    {
        BulletsPool.Instance.Destroy();
        ExplosionsPool.Instance.Destroy();
    }
}
