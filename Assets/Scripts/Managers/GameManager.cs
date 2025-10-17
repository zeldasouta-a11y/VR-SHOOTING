using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//UnityEvent Inspectror�Őݒ�\
[System.Serializable]
public class GameModeChangedEvent : UnityEvent<GameManager.GameMode> { }

public class GameManager : MonoBehaviour
{
    public enum GameMode
    {
        VeryEasy = 1,
        Easy = 2,
        Normal = 3,
        Hard = 4,
        VeryHard = 5,
        
    }
    [Serializable]
    public class GameModeSetting
    {
        public GameMode GameMode;
        public int indexMin;
        public int indexMax;
        [Header("Target Setting")]
        public float createduretion;
        public Vector3 minPos;
        public Vector3 maxPos;
        public Vector3 moveVec;
        [Header("Gun Setting")]
        public int fireRate;
        public int ReloadConstant;
    }
    [Header("��Փx�ɂ���ĕς��")]
    [SerializeField] private float createDuration = 1.0f;
    [SerializeField] private int indexMin = 0;
    [SerializeField] private int indexMax = 1;
    [Header("���[�h���Ƃ̐ݒ�l")]
    [SerializeField] private float GameTimeLimit = 100.0f;
    [SerializeField] private float phase1Time = 30.0f;
    [SerializeField] private float phase2Time = 30.0f;
    [SerializeField] private float phase3Time = 40.0f;
    public List<GameModeSetting> modeSettings = new List<GameModeSetting>();
    [Header("other")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] float fullAutoDuration = 20.0f;
    [SerializeField] private bool isFullAutoMode = false;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLimitText;
    [SerializeField] GameMode gameMode;
    bool isGameStart = false;
    private float limitTimer = 0.0f;
    private float creareTimer = 0.0f;

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
            if (gameMode != value)
            {
                gameMode = value;
                //Event trigger
                OnGameModeChanged.Invoke(gameMode);
                //OnGameProgressChanged.Invoke(gameMode);
            }
        }
    }
    //UnityEvent
    public GameModeChangedEvent OnGameModeChanged;
    //System Event
    public event Action<bool> OnFullAutoChanged;
    private void Start()
    {
        OnGameModeChanged.AddListener(OnEnumChanedHandle);
        //OnGameModeChanged += OnEnumChangedHandle;
        //�r���h���ɂ͂���
        //gameMode = GameMode.Normal;
        OnEnumChanedHandle(gameMode);
        creareTimer = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isGameStart) return;

        creareTimer += Time.deltaTime;
        if (creareTimer > createDuration)
        {
            ManagerLocator.Instance.CreateTarget.CreateInstanceAndSetCameraAndScripts(UnityEngine.Random.Range(indexMin, indexMax));
            creareTimer = 0.0f;
        }
    }

    public void OnEnumChanedHandle(GameMode mode)
    {
        var setting = modeSettings.Find(s => s.GameMode == mode);
        if (setting != null)
        {
            createDuration = setting.createduretion;
            indexMin = setting.indexMin;
            indexMax = setting.indexMax;
        }
        else
        {
            Debug.LogWarning($"GameMode {mode} �̐ݒ肪������܂���");
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
        StartCoroutine(GameTimer());
        ManagerLocator.Instance.CreateTarget.CreateInstanceAndSetCameraAndScripts(UnityEngine.Random.Range(indexMin, indexMax));
    }
    private IEnumerator GameTimer()
    {
        isGameStart = true;
        for (limitTimer = phase1Time; limitTimer >= 0; limitTimer -= 0.1f)
        {
            timeLimitText.text = limitTimer.ToString("n1");
            yield return new WaitForSeconds(.1f); ;
        }
        timeLimitText.text = 0.ToString();
        //ここで、難易度変更
        yield return new WaitForSeconds(3f);
        
        int nextMode = ((int)Mode + 1);
        Debug.Log($"NextMode:{(GameMode)nextMode}");

        Mode = (nextMode <= 5) ? (GameMode)nextMode : GameMode.VeryHard;
        for (limitTimer = phase2Time; limitTimer >= 0f; limitTimer -= 0.1f)
        {
            timeLimitText.text = limitTimer.ToString("n1");
            yield return new WaitForSeconds(.1f);
        }
        timeLimitText.text = 0.ToString();
        //ここで、難易度変更
        yield return new WaitForSeconds(3f);
        for (limitTimer = phase3Time; limitTimer >= 0f; limitTimer -= 0.1f)
        {
            timeLimitText.text = limitTimer.ToString("n1");
            yield return new WaitForSeconds(.1f);
        }
        isGameStart= false;
    }
    [OnInspectorButton]
    public void ChangeMode(GameMode nextmode)
    {
        Mode = nextmode;
    }
    public void AddScore(int point)
    {
        totalScore += point;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
        Debug.Log("Score: " + totalScore);
    }

}
