using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [Header("other")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] float fullAutoDuration = 20.0f;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLimitText;
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
        
        if(gamestate == GameState.Playing)
        {
            return;
        }
        gamestate = GameState.Playing;
        OnGameStart?.Invoke();
    }

    public void GameEnd()
    {
        timeLimitText.text = "End!";

        gamestate = GameState.Ended;
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
        totalScore += point;
        
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
