using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//UnityEvent Inspectrorで設定可能
[System.Serializable]
public class GameModeChangedEvent : UnityEvent<GameManager.GameMode> { }

public class GameManager : MonoBehaviour
{
    public enum GameMode
    {
        VeryEasy = 0,
        Easy = 1<<0,
        Normal = 1<<1,
        Hard = 1<<2,
        VeryHard = 1<<3,
        
    }
    [Serializable]
    public class GameModeSetting
    {
        public GameMode GameMode;
        public float createduretion;
        public int indexMin;
        public int indexMax;
    }
    [Header("難易度によって変わる")]
    [SerializeField] private float createDuration = 1.0f;
    [SerializeField] private int indexMin = 0;
    [SerializeField] private int indexMax = 1;
    [Header("モードごとの設定値")]
    public List<GameModeSetting> modeSettings = new List<GameModeSetting>();
    [Header("other")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] float fullAutoDuration = 20.0f;

    [SerializeField] private bool isFullAutoMode = false;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameMode gameMode;
    private float timer = 0.0f;

    public bool IsFullAutoMode
    {
        get => isFullAutoMode;
        set
        {
            if (isFullAutoMode != value)
            {

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
        //ビルド時にはいる
        //gameMode = GameMode.Normal;
        OnEnumChanedHandle(gameMode);
        timer = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > createDuration)
        {
            ManagerLocator.Instance.CreateTarget.CreateInstanceAndSetCameraAndScripts(UnityEngine.Random.Range(indexMin, indexMax));
            timer = 0.0f;
        }
        FullAutoMode();
    }
    private void Foo()
    {
        Debug.Log("Mode Changed!");
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
            Debug.LogWarning($"GameMode {mode} の設定が見つかりません");
        }
        switch (gameMode)
        {
            case GameMode.VeryEasy:
                break;
            case GameMode.Easy:
                Foo();
                break;
            case GameMode.Normal:
                Foo();
                break;
            case GameMode.Hard:
                Foo();
                break;
            case GameMode.VeryHard:
                Foo();
                break;
        }
    }
    [OnInspectorButton("",true)]
    public void StartFullAuto()
    {
        StartCoroutine(FullAutoMode());
    }
    private IEnumerator FullAutoMode()
    {
        isFullAutoMode = true;
        yield return new WaitForSeconds(fullAutoDuration);
        isFullAutoMode = false;
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
