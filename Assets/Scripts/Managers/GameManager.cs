using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Collections;
using UnityEngine;


public enum GameState { Idle, Playing, Paused, Ended }
public enum Tutorial { Use, Skip };

//UnityEvent Inspectror�Őݒ�\
[System.Serializable]

public class GameManager : MonoBehaviour
{
    
    

    [Header("Seed")]
    [SerializeField] bool isCustomSeed = true;
    [EnableIf("isCustomSeed",hideWhenFalse: false)]
    [SerializeField] int gameSeed = 12345;
    public int GameSeed => gameSeed;

    [Header("BGM Setting")]
    [SerializeField] AudioSource fullAutoBGM;
    [SerializeField] AudioSource enddingBGM;
    [Header("other")]
    [EnableIf("isFullAutoMode",hideWhenFalse:false)]
    [SerializeField] private int totalScore = 0;
    public int TotalScore => totalScore;
    [SerializeField] float fullAutoDuration = 35.0f;
    [Header("チュートリアルが有効かどうか")]
    [SerializeField] Tutorial tutorial;
    public Tutorial Tutorial => tutorial;
    Dictionary<string, int> targetHitCount = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> TargetHitDict => targetHitCount;
    private GameState gamestate = GameState.Idle;
    public GameState State => gamestate;
    private bool isFullAutoMode = false;
    

    //System Event
    public event Action<bool> OnFullAutoChanged;
    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action OnHit;
    private void Start()
    {
        StartGame();
    }
    private void OnDisable()
    {
        var phase = ManagerLocator.Instance.Phase;
        if(phase != null)
        {
            phase.OnAllPhaseEnd -= GameEnd;
        }
    }
    public void SetEvent(PhaseManager phase)
    {
        phase.OnAllPhaseEnd += GameEnd;
    }
    [OnInspectorButton("Game Restart", true)]
    private void GameReStart(bool sameSeed)
    {
        enddingBGM.Stop();
        isCustomSeed = sameSeed;
        StartGame();
    }
    [OnInspectorButton("", true)]
    public void StartFullAuto()
    {
        if(!isFullAutoMode)
        StartCoroutine(FullAutoMode());
    }
    private IEnumerator FullAutoMode()
    {
        isFullAutoMode = true;
        fullAutoBGM.Play();
        OnFullAutoChanged?.Invoke(true);
        yield return new WaitForSeconds(fullAutoDuration);
        fullAutoBGM.Stop();
        OnFullAutoChanged?.Invoke(false);
        isFullAutoMode = false;
    }

    private void StartGame()
    {
        if (!isCustomSeed)
        {
            long tick = DateTime.Now.Ticks;
            gameSeed = (int)tick;
        }
        if(gamestate == GameState.Playing)
        {
            return;
        }
        gamestate = GameState.Playing;
        OnGameStart?.Invoke();
    }

    public void GameEnd()
    {
        Debug.Log("GameEnd!");
        gamestate = GameState.Ended;
        enddingBGM.Play();
        fullAutoBGM?.Stop();
        OnGameEnd?.Invoke();
    }
    
    public void AddScore(int point,string name)
    {
        if(!ManagerLocator.Instance.Phase.IsIgnoreScoreMode)
        {
            totalScore += point;
            if (targetHitCount.TryGetValue(name, out int count))
            {
               targetHitCount[name] = count + 1;
            }
            else
            {
                targetHitCount[name] = 1;
            }
        }
        
        
        OnHit?.Invoke();
    }
}
