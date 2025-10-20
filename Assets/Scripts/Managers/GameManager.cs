using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public enum GameMode { Tutorial,Phase1, Phase2, Phase3, Phase4, Phase5 }
public enum GameState { Idle, Playing, Paused, Ended }

//UnityEvent Inspectror�Őݒ�\
[System.Serializable]

public class GameManager : MonoBehaviour
{
    
    [Serializable]
    public class GamePhaseSetting
    {
        public GameMode gameMode;
        public float phaseTime;
        public bool hasExitTime;
        [Header("Target Setting")]
        public float createduretion;
        public TargetDataSO targetSettingSO;
        //[Header("Gun Setting")]
        //public int fireRate;
        //public int ReloadConstant;

    }
    [Header("��Փx�ɂ���ĕς��")]
    [SerializeField] private float createDuration = 1.0f;
    [Header("���[�h���Ƃ̐ݒ�l")]
    [SerializeField] int gameSeed = 12345; 
    public int GameSeed => gameSeed;
    //[SerializeField] private float GameTimeLimit = 100.0f;
    [SerializeField] private float phaseChangeTime = 3.0f;
    [SerializeField] List<GamePhaseSetting> phaseSettings;
    [Header("other")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] float fullAutoDuration = 20.0f;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLimitText;
    [SerializeField] GameMode gameMode;
    [SerializeField] bool IsTurorial = true;
    Dictionary<string, int> targetHitCount = new Dictionary<string, int>();
    GameState gamestate = GameState.Idle;
    private float limitTimer = 0.0f;
    private float createTimer = 0.0f;
    private bool isFullAutoMode = false;
    private bool hasExitTime = false;
    private Dictionary<GameMode,GamePhaseSetting> GameSettingDic = new Dictionary<GameMode, GamePhaseSetting>();
    //Resolve GC(Overhead)
    private static readonly WaitForSeconds wait01 = new WaitForSeconds(0.1f);
    private WaitForSeconds waitPhaseChange;

    public bool IsFullAutoMode
    {
        get => isFullAutoMode;
        set
        {
            if (isFullAutoMode != value)
            {
                OnFullAutoChanged?.Invoke(value);
                isFullAutoMode = value;
            }
        }
    }


    public GameMode Mode
    {
        get => gameMode;
        set
        {
            gameMode = value;
            //Event trigger
            OnGameModeChanged.Invoke(GameSettingDic[gameMode]);
            Debug.Log($"NextMode:{value.ToString()}");
            
        }
    }
    //System Event
    public event Action<GamePhaseSetting> OnGameModeChanged;
    public event Action<bool> OnFullAutoChanged;
    public event Action OnCreateTime;
    private void Start()
    {
        OnGameModeChanged += OnEnumChanedHandle;
        SetupDic();
        waitPhaseChange = new WaitForSeconds(phaseChangeTime);
        //gameMode = GameMode.Normal;
        OnEnumChanedHandle(GameSettingDic[gameMode]);
        createTimer = 0f;
        StartGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (gamestate != GameState.Playing) return;
        if (Mode == GameMode.Tutorial) return;
        createTimer += Time.deltaTime;
        if (createTimer > createDuration)
        {
            OnCreateTime?.Invoke();
            createTimer = 0.0f;
        }
    }
    private void OnDisable()
    {
        OnGameModeChanged -= OnEnumChanedHandle;
    }
    private void SetupDic()
    {
        foreach (GamePhaseSetting key in phaseSettings)
        {
            GameSettingDic[key.gameMode] = key;
        }
    }
    public void OnEnumChanedHandle(GamePhaseSetting modeSetting)
    {
        if (modeSetting != null)
        {
            createDuration = modeSetting.createduretion;
            //indexMin = setting.indexMin;
            //indexMax = setting.indexMax;
        }
        else
        {
            Debug.LogWarning($"gameMode {modeSetting.gameMode} �̐ݒ肪������܂���");
        }
    }
    [OnInspectorButton("",true)]
    public void StartFullAuto()
    {
        StartCoroutine(FullAutoMode());
    }
    private IEnumerator FullAutoMode()
    {
        IsFullAutoMode = true;
        yield return new WaitForSeconds(fullAutoDuration);
        IsFullAutoMode = false;
    }
    [OnInspectorButton("",true)]
    public void StartGame()
    {
        if(gamestate == GameState.Playing)
        {
            return;
        }
        StartCoroutine(GameTimer());
        OnCreateTime?.Invoke();
    }
    public void EndTutorial()
    {
        IsTurorial = false;
    }
    private IEnumerator GameTimer()
    {
        gamestate = GameState.Playing;
        foreach (var phase in phaseSettings)
        {
            Mode = phase.gameMode;
            if(!phase.hasExitTime)
            {
                yield return RunPhaseForWait();
            }
            else
            {
                yield return RunPhase(phase);
            }
            
            yield return waitPhaseChange;
        }
        timeLimitText.text = "End!";

        gamestate = GameState.Ended;
    }
    private IEnumerator RunPhase(GamePhaseSetting phase)
    {
        
        for (limitTimer = phase.phaseTime;limitTimer >=0;limitTimer -= 0.1f)
        {
            UpdateUI();
            yield return wait01;
        }
        UpdateUI();
    }
    private IEnumerator RunPhaseForWait()
    {
        //WaitUntil() Falseの間待機
        //WaitWhile() Trueの間待機
        yield return new WaitWhile(() => IsTurorial);
    }
    private void UpdateUI()
    {
        timeLimitText.text = limitTimer.ToString("n1");
    }
    [OnInspectorButton]
    public void ChangeMode(GameMode nextmode)
    {
        Mode = nextmode;
    }
    public void AddScore(int point,string name)
    {
        totalScore += point;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
        Debug.Log("Score: " + totalScore);
        if (targetHitCount.TryGetValue(name,out int count))
        {
            targetHitCount[name] = count+1;
        }
        else
        {
            targetHitCount[name] = 1;
        }
        
    }


}
