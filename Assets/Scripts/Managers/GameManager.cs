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
    [SerializeField] int gameSeed = 12345;
    public int GameSeed => gameSeed;

    [Header("UI")]
    [SerializeField] GameObject enddingPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLimitText;
    [SerializeField] AudioSource enddingBGM;
    [Header("other")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] float fullAutoDuration = 20.0f;
    
    [SerializeField] Tutorial tutorial;
    public Tutorial Tutorial => tutorial;
    Dictionary<string, int> targetHitCount = new Dictionary<string, int>();
    [SerializeField] GameState gamestate = GameState.Idle;
    public GameState State => gamestate;
    private bool isFullAutoMode = false;
    

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

    //System Event
    
    public event Action<bool> OnFullAutoChanged;

    public event Action OnGameStart;
    public event Action OnHit;
    private void Start()
    {
        StartGame();
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
        enddingPanel.SetActive(false);
        if(gamestate == GameState.Playing)
        {
            return;
        }
        gamestate = GameState.Playing;
        OnGameStart?.Invoke();
    }

    public void GameEnd()
    {
        gamestate = GameState.Ended;
        timeLimitText.text = "End!";
        enddingBGM.Play();
        ShowResult();
    }
    private void ShowResult()
    {
        resultText.text = "";
        resultText.text += "スコア: " + totalScore.ToString() + "\n";
        foreach (var item in targetHitCount)
        {
            if (item.Key == "TutorialTerget") continue;
            resultText.text += item.Key + ": " + item.Value.ToString() + "\n";
        };
        enddingPanel.SetActive(true);

    }
    public void UpdateUI(float limitTimer)
    {
        if(timeLimitText != null)
        {
            timeLimitText.text = limitTimer.ToString("n1");
        }
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
    }
    
    public void AddScore(int point,string name)
    {
        if(ManagerLocator.Instance.Phase.Phase != PhaseState.Bonus)
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
