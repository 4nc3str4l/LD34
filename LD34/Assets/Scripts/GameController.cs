using UnityEngine;

public class GameController : MonoBehaviour {

    public const int NUM_DEADS_TO_WIN = 1000;

    int _numDeads = 0;
    public int numDeads {get { return _numDeads; }}

    public static GameController Instance;

    private const float SPAWN_EVERY = 15f;
    private const int CAR_PROBABILITY = 100;
    private const float SPAWN_CAR_INTERVAL = 15f;

    public MonsterController Monster;
    private GameObject _originalHuman;
    private GameObject _originalSoldier;
    private GameObject _originalCar;
    private float _lastExplored = 0;
    private float _lastSpawned = 0;
    private float _lastCarSpawned = 0;

    void Awake()
    {
        Instance = this;
        Monster = GameObject.Find("MonsterContainer").GetComponentInChildren<MonsterController>();
        _originalHuman = Resources.Load<GameObject>("Prefabs/Human");
        _originalSoldier = Resources.Load<GameObject>("Prefabs/Soldier");
        _originalCar = Resources.Load<GameObject>("Prefabs/Car");
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
                if (UnityEngine.Random.Range(0, 100) > 30)
                {
                    GameObject.Instantiate(_originalSoldier, new Vector2(_lastSpawned + i, y), Quaternion.identity);
                }
                else
                {
                    GameObject.Instantiate(_originalHuman, new Vector2(_lastSpawned + i, y), Quaternion.identity);
                }
            }
        }

        if (Time.time - _lastCarSpawned >= SPAWN_CAR_INTERVAL)
        {
            int dice = UnityEngine.Random.Range(0, CAR_PROBABILITY);
            if (dice == 0 || dice == CAR_PROBABILITY - 1)
            {
                float offset = (width + SPAWN_EVERY) * (dice == 0 ? 1 : -1);
                GameObject car = (GameObject)GameObject.Instantiate(_originalCar);
                car.transform.position = new Vector2(Monster.transform.position.x + offset, 0.1f);
                car.GetComponentInChildren<Mob>().StrikeDirection = (dice == 0 ? Vector2.left : Vector2.right);

                _lastCarSpawned = Time.time;
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
