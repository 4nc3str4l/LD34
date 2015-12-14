using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUIController : MonoBehaviour {

    public enum ChooseAnimationState {PREPARING, STOPPED, SHOWING, HIDDING, MERGING, CHOOSING, FINISHING };
    const float SPAWN_RATE = 0.4f;
    const float TIME_BETWEEN_MERGES = 0.5f;
    const float MERGE_TIMES = 6;

    const string PREPARE = "You will get lost MUAHAHA";
    const string REMEMBER = "REMEMBER";
    const string FOLLOW_WITH_YOUR_EYES = "Look with those lazy eyes";
    const string CHOOSE = "Choose 2 skills";

    float _nextSpawn = 0;

    public static GUIController instance;
    public Text deadPannelText, deadPannelTitle;
    private Text _numDeadsText, _chooserText;


    public BtnSkill leftSkill, rightSkill;
    public BtnSkill leftSkillOverlay, rightSkillOverlay;
    public List<GameObject> skills;
    public List<GameObject> choosePosition;
    public List<Vector3> _avaliablePositions;
    public GameObject skillChosser;

    Animator _animator;
    AudioSource _audioSource;
    public AudioClip deadAnimationSound, cluck, welcome, selection;

    public Sprite defaultSprite;

    List<int> _positions, _destinationPositions;

    ChooseAnimationState _actualChooseAnimationState;

    public ChooseAnimationState actualChooseAnimationState { get { return _actualChooseAnimationState; } }

    int i = 0;
    int _timesMerged = 0;

    bool firstChoose = true;

    float hideTutorialTime =  0f;

    void Awake()
    {
        instance = this;
        _numDeadsText = transform.Find("DeadCounter").GetComponent<Text>();
        _chooserText = GameObject.Find("ChooserText").GetComponent<Text>();
        _positions = new List<int>();
        _destinationPositions = new List<int>();
        _avaliablePositions = new List<Vector3>();
        _actualChooseAnimationState = ChooseAnimationState.STOPPED;
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _nextSpawn = 0;
        foreach(GameObject pos in choosePosition)
        {
            _avaliablePositions.Add(pos.transform.localPosition);
        }
        showChooseAbilityAnimation();
    }

	void Update () {

        if(_animator.GetInteger("TUTORIAL") == 1 && Time.time > hideTutorialTime)
        {
            _animator.SetInteger("TUTORIAL", 0);
        }
        if (_actualChooseAnimationState != ChooseAnimationState.STOPPED)
        {
            updateSkillChooser();
        }
    }

    private void updateSkillChooser()
    {
        switch (_actualChooseAnimationState)
        {
            case ChooseAnimationState.PREPARING:
                if (AbilityController.Instance.Abilities[AbilityType.PROTECTION_FIELD].IsEnabled)
                {
                    AbilityController.Instance.Abilities[AbilityType.PROTECTION_FIELD].OnEnd();
                }
                GameController.Instance.Monster.GetComponentInChildren<MonsterFX>().enableAura();

                skillChosser.SetActive(true);
                _positions = new List<int>();
                _destinationPositions = new List<int>();
                _avaliablePositions = new List<Vector3>();
                _nextSpawn = 0;
                i = 0;
                foreach (GameObject pos in choosePosition)
                {
                    pos.SetActive(true);
                    _avaliablePositions.Add(pos.transform.localPosition);
                }
                _actualChooseAnimationState = ChooseAnimationState.SHOWING;
                setPositionsToChoose();
                _chooserText.text = PREPARE;
                _animator.SetInteger("STATE", 2);
                if(firstChoose)
                {
                    _audioSource.PlayOneShot(welcome);
                }
                else
                {
                    _audioSource.PlayOneShot(selection);
                }
                break;
            case ChooseAnimationState.SHOWING:
                if (_nextSpawn < Time.time)
                {
                    int selected_pos = popPosition();
                    choosePosition[i].transform.GetComponent<Image>().sprite = skills[selected_pos].GetComponent<Image>().sprite;
                    choosePosition[i].transform.GetComponent<ChooseButtonSkill>().AssociateAbility(skills[selected_pos].GetComponent<BtnSkill>().ability);
                    _nextSpawn = Time.time + SPAWN_RATE;
                    i++;
                    if (i == skills.Count)
                    {
                        _actualChooseAnimationState = ChooseAnimationState.HIDDING;
                        i = 0;
                        setPositionsToChoose();
                        _chooserText.text = REMEMBER;
                    }
                }
                break;
            case ChooseAnimationState.HIDDING:
                if (_nextSpawn < Time.time)
                {
                    choosePosition[popPosition()].transform.GetComponent<Image>().sprite = defaultSprite;
                    _audioSource.PlayOneShot(cluck, 1f);
                    _nextSpawn = Time.time + SPAWN_RATE;
                    i++;
                    if (i == skills.Count)
                    {
                        _actualChooseAnimationState = ChooseAnimationState.MERGING;
                        setPositionsToChoose();
                        setDestinationPositionsToChoose();
                        _nextSpawn = Time.time + TIME_BETWEEN_MERGES;
                        _chooserText.text = FOLLOW_WITH_YOUR_EYES;
                        i = 0;
                    }
                }
                break;
            case ChooseAnimationState.MERGING:

                if(i < skills.Count)
                {
                    choosePosition[popPosition()].GetComponent<ChooseButtonSkill>().moveToPosition(_avaliablePositions[popDestinationPosition()]);
                }

                i++;

                if(_nextSpawn < Time.time)
                {
                    i = 0;
                    _timesMerged++;
                    setPositionsToChoose();
                    setDestinationPositionsToChoose();
                    _nextSpawn = Time.time + TIME_BETWEEN_MERGES;
                }

                if(i == skills.Count)
                {
                    i = skills.Count;
                }

                if (_timesMerged == MERGE_TIMES)
                {
                    _timesMerged = 0;
                    _actualChooseAnimationState = ChooseAnimationState.CHOOSING;
                    _chooserText.text = CHOOSE;
                    i = 0;
                }
                break;
            case ChooseAnimationState.CHOOSING:
                if(i == 2)
                {
                    _actualChooseAnimationState = ChooseAnimationState.FINISHING;
                }
                break;
            case ChooseAnimationState.FINISHING:
                _actualChooseAnimationState = ChooseAnimationState.STOPPED;
                for(int x = 0; x  < skills.Count; x++)
                {
                    choosePosition[x].transform.localPosition.Set(_avaliablePositions[x].x, _avaliablePositions[x].y, _avaliablePositions[x].z);

                }
                if (firstChoose)
                {
                    _animator.SetInteger("TUTORIAL", 1);
                    hideTutorialTime = Time.time + 20f;
                    firstChoose = false;
                }
    
                _animator.SetInteger("STATE", 0);

                GameController.Instance.Monster.GetComponentInChildren<MonsterFX>().disableAura();
                break;
            default:
                break;
        }
    }

    public void updateDeadCounter()
    {
        _numDeadsText.text = GameController.Instance.numDeads.ToString();
        deadPannelText.text = GameController.Instance.numDeads.ToString();
        _audioSource.PlayOneShot(deadAnimationSound);
        if (_actualChooseAnimationState == ChooseAnimationState.STOPPED)
        {
            _animator.SetInteger("STATE", 1);
            Invoke("returnToNormalState", 0.35f);
        }
    }

    private void setPositionsToChoose()
    {
        for(int i = 0; i < skills.Count; i++)
        {
            _positions.Add(i);
        }
    }

    private int popPosition()
    {
        int posToReturn = -1;
        if(_positions.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, _positions.Count);
            posToReturn = _positions[i];
            _positions.RemoveAt(i);
        }
        return posToReturn;
    }


    private void setDestinationPositionsToChoose()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            _destinationPositions.Add(i);
        }
    }

    private int popDestinationPosition()
    {
        int posToReturn = -1;
        if (_destinationPositions.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, _destinationPositions.Count);
            posToReturn = _destinationPositions[i];
            _destinationPositions.RemoveAt(i);
        }
        return posToReturn;
    }

    public void chooseSkill(AbilityType ability, AudioClip c)
    {
        GetComponent<AudioSource>().PlayOneShot(c, 1f);   
        if(_actualChooseAnimationState == ChooseAnimationState.CHOOSING)
        {
            BtnSkill skillInfo = null;
            foreach(GameObject skill in skills)
            {
                if(skill.GetComponent<BtnSkill>().ability == ability)
                {
                    skillInfo = skill.transform.GetComponent<BtnSkill>();
                    break;
                }
            }

            if(i == 0)
            {
                AbilityController.Instance.BoundAtLeft = AbilityController.Instance.Abilities[ability];
                leftSkill.GetComponent<BtnSkill>().setNewInfo(skillInfo);
                leftSkillOverlay.GetComponent<BtnSkill>().setNewInfo(skillInfo);
            }
            else
            {
                AbilityController.Instance.BoundAtRight = AbilityController.Instance.Abilities[ability];
                rightSkill.GetComponent<BtnSkill>().setNewInfo(skillInfo);
                rightSkillOverlay.GetComponent<BtnSkill>().setNewInfo(skillInfo);
            }
            i++;
        }
    }

    void returnToNormalState()
    {
        if(_actualChooseAnimationState == ChooseAnimationState.STOPPED) _animator.SetInteger("STATE", 0);
    }

    public void showDeadAnimation()
    {
        _animator.SetInteger("STATE", 3);
        
    }

    public void showWinAnimation()
    {
        deadPannelTitle.text = "You are mad!!";
        _animator.SetInteger("STATE", 3);
    }

    public void retryBtn()
    {
        Application.LoadLevel("C_GameScene");
    }

    public void mainMenuBtn()
    {
        Application.LoadLevel("MainScene");
    }

    public void showChooseAbilityAnimation()
    {
        _actualChooseAnimationState = ChooseAnimationState.PREPARING;
    }
}
