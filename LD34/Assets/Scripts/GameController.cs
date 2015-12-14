using UnityEngine;

public class GameController : MonoBehaviour {

    public const int NUM_DEADS_TO_WIN = 10;

    int _numDeads = 0;
    public int numDeads {get { return _numDeads; }}

    public static GameController instance;

    private const float SPAWN_EVERY = 15f;

    private MonsterController _monster;
    private GameObject _originalHuman;
    private GameObject _originalSoldier;
    private float _lastExplored = 0;
    private float _lastSpawned = 0;

    void Awake()
    {
        instance = this;
        _monster = GameObject.Find("MonsterContainer").GetComponentInChildren<MonsterController>();
        _originalHuman = Resources.Load<GameObject>("Prefabs/Human");
        _originalSoldier = Resources.Load<GameObject>("Prefabs/Soldier");
    }

    void Update()
    {
        float width = Camera.main.orthographicSize * Screen.width / Screen.height;
        _lastExplored = Mathf.Max(_lastExplored, _monster.transform.position.x + width / 2);

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
    }

    public void OnDisable()
    {
        BulletsPool.Instance.Destroy();
        ExplosionsPool.Instance.Destroy();
    }
}
