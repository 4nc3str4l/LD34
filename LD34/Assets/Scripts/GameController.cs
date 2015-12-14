﻿using UnityEngine;

public class GameController : MonoBehaviour {

    public const int NUM_DEADS_TO_WIN = 200;

    int _numDeads = 0;
    public int numDeads {get { return _numDeads; }}

    public static GameController Instance;

    private const float MAX_SPAWN = 50f;
    private const float SPAWN_EVERY = 15f;
    private const int CAR_PROBABILITY = 100;
    private const float SPAWN_CAR_INTERVAL = 15f;

    private float spawnRange = SPAWN_EVERY;
    private int spawnDirection = 1;

    public GameObject Car;

    public MonsterController Monster;
    private GameObject _originalHuman;
    private GameObject _originalSoldier;
    private GameObject _originalCar;
    private float _lastExplored = float.NegativeInfinity;
    private float _lastSpawned = float.NegativeInfinity;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.Instance.ToggleState();
        }

        if (GUIController.instance.actualChooseAnimationState != GUIController.ChooseAnimationState.STOPPED ||
            PauseMenu.Instance.State != PauseMenu.PauseState.NOT_PAUSED)
        {
            return;
        }

        float width = Camera.main.orthographicSize * Screen.width / Screen.height;

        if (spawnDirection == 1)
        {
            _lastExplored = Mathf.Max(_lastExplored, Monster.transform.position.x + (width * spawnDirection) / 2);
        }
        else
        {
            _lastExplored = Mathf.Min(_lastExplored, Monster.transform.position.x + (width * spawnDirection) / 2);
        }

        if ((spawnDirection == 1 && _lastExplored + spawnRange > Constants.MAP_MAX_X) ||
            (spawnDirection == -1 && _lastExplored + spawnRange < Constants.MAP_MIN_X))
        {
            spawnDirection *= -1;
            spawnRange *= -1;
        }

        if ((spawnDirection == 1 && _lastExplored + spawnRange > _lastSpawned + spawnRange / 2) ||
            (spawnDirection == -1 && _lastExplored + spawnRange < _lastSpawned + spawnRange / 2))
        {
            _lastSpawned = _lastExplored + spawnRange;

            int numSpawns = (int)(_numDeads * (float)MAX_SPAWN / NUM_DEADS_TO_WIN);

            for (int i = 0; i < 5; ++i)
            {
                float y = UnityEngine.Random.Range(0.5f, 15);
                if (UnityEngine.Random.Range(0, 100) > 30)
                {
                    GameObject.Instantiate(_originalSoldier, new Vector2(_lastSpawned + i * spawnDirection, y), Quaternion.identity);
                }
                else
                {
                    GameObject.Instantiate(_originalHuman, new Vector2(_lastSpawned + i * spawnDirection, y), Quaternion.identity);
                }
            }
        }

        if (Time.time - _lastCarSpawned >= SPAWN_CAR_INTERVAL && _lastExplored > -30)
        {
            int dice = UnityEngine.Random.Range(0, CAR_PROBABILITY);
            if (dice == 0 || dice == CAR_PROBABILITY - 1)
            {
                float offset = (width + spawnRange) * (dice == 0 ? 1.5f : -1.5f);
                Car = (GameObject)GameObject.Instantiate(_originalCar);
                Car.transform.position = new Vector2(Monster.transform.position.x + offset, 0.1f);
                Car.GetComponentInChildren<Mob>().StrikeDirection = (dice == 0 ? Vector2.left : Vector2.right) * spawnDirection;

                _lastCarSpawned = Time.time;
            }
        }
    }

    public void addDead()
    {
        _numDeads++;
        GUIController.instance.updateDeadCounter();

        if (_numDeads % 30 == 0 && GUIController.instance.actualChooseAnimationState == GUIController.ChooseAnimationState.STOPPED)
        {
            GUIController.instance.showChooseAbilityAnimation();
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
