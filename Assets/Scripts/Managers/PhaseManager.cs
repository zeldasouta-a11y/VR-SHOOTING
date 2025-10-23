using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using static GameManager;

public class PhaseManager : MonoBehaviour
{

    [SerializeField] float phaseChangeTime = 3.0f;
    [SerializeField] float fixedUpdate = 0.1f;
    [SerializeField] PhaseState nowGamePhase;
    [SerializeField] bool isEndPhase = true;
    [SerializeField] private List<PhaseSettingData> phaseSettings;
    private PhaseSettingData nowPhaseSetting;
    private Dictionary<PhaseState,PhaseSettingData> phaseToSettingDic = new Dictionary<PhaseState, PhaseSettingData>();
    private Dictionary<PhaseState, int> phaseToIndexDict = new Dictionary<PhaseState, int>();
    private Dictionary<PhaseState,int> phaseHitCountDict = new Dictionary<PhaseState,int>();
    //Resolve GC(Overhead)
    private static WaitForSeconds waitUpdate ;
    private static WaitForSeconds waitPhaseChange;
    private int phaseHitCount = 0;
    private int phaseIndex = 0;
    public PhaseState Phase
    {
        get => nowGamePhase;
        private set
        {
            nowGamePhase = value;
            //Event trigger
            if (phaseToSettingDic.TryGetValue(nowGamePhase, out var setting))
            {
                OnGamePhaseChanged?.Invoke(setting);
                phaseHitCount = 0;
                Debug.Log($"NextMode:{value.ToString()}");
            }
            else
            {
                Debug.LogWarning($"[PhaseManager] Undefined phase: {nowGamePhase}");
            }
        }
    }
    public event Action<PhaseSettingData> OnGamePhaseChanged;
    public event Action OnCreateTime;
    public event Action OnPhaseEnd;
    public void SetEvent(GameManager gameManager)
    {
        gameManager.OnGameStart += OnGameStartHandle;
        gameManager.OnHit += OnHitHandle;
        OnGamePhaseChanged += OnPhaseChanedHandle;
        
    }
    private void Start()
    {
       
        waitPhaseChange = new WaitForSeconds(phaseChangeTime);
        waitUpdate = new WaitForSeconds(fixedUpdate);
        isEndPhase = (ManagerLocator.Instance.Game.Tutorial == Tutorial.Use);
        
    }
    private void OnDisable()
    {
        var gameManager = ManagerLocator.Instance.Game;
        if(gameManager != null)
        {
            gameManager.OnGameStart -= OnGameStartHandle;
            gameManager.OnHit -= OnHitHandle;
        }
        
        OnGamePhaseChanged -= OnPhaseChanedHandle;
    }
    private void OnGameStartHandle()
    {
        SetupDic(phaseSettings);
        StartCoroutine(GamePhaseTimer());

    }
    private void OnPhaseChanedHandle(PhaseSettingData phaseSetting)
    {
        if (phaseSetting != null)
        {
            nowPhaseSetting = phaseSetting;
        }
        else
        {
            Debug.LogWarning($"nowGamePhase {phaseSetting.gamePhase} is not Setting");
        }
    }
    private IEnumerator GamePhaseTimer()
    {
        for(phaseIndex = 0; phaseIndex < phaseSettings.Count ;phaseIndex++)
        {
            var phase = phaseSettings[phaseIndex];
            
            Phase = phase.gamePhase;
            StartCoroutine(CreateTimer(phase));
            if (!phase.hasExitTime)
            {
                isEndPhase = false;
                yield return RunWaitForEndPhase();
            }
            else
            {
                yield return RunPhase(phase);
            }
            OnPhaseEnd?.Invoke();
            yield return waitPhaseChange;
        }
        ManagerLocator.Instance.Game.GameEnd();
    }
    private IEnumerator RunPhase(PhaseSettingData phase)
    {
        //StartCoroutine(CreateTimer(phase));
        for (float limitTimer = phase.phaseTime; limitTimer >= 0; limitTimer -= fixedUpdate)
        {
            ManagerLocator.Instance.Game.UpdateUI(limitTimer);
            yield return waitUpdate;
        }
        
        ManagerLocator.Instance.Game.UpdateUI(0.0f);
    }
    private IEnumerator RunWaitForEndPhase()
    {
        
        //WaitUntil() Falseの間待機
        //WaitWhile() Trueの間待機
        yield return new WaitWhile(() => !isEndPhase);
    }
    private IEnumerator CreateTimer(PhaseSettingData phase)
    {
        float totalTime = 0.0f;
        yield return null;//nullを回避するため待つ
        if (phase.spawnType == SpawnType.Time)
        {
            for(float timer = phase.createDuration; totalTime <= phase.phaseTime; timer += fixedUpdate,totalTime += fixedUpdate)
            {
                if (timer >= phase.createDuration)
                {
                    for(int i = 0; i < phase.onSpawnCreateCount; i++)
                    {
                        OnCreateTime?.Invoke();
                        yield return null;
                    }
                    timer = 0.0f;
                }
                yield return waitUpdate;
            }
            
        }
        else if(phase.spawnType == SpawnType.Count)
        {
            for(int i = 0; i < phase.onSpawnCreateCount; i++)
            {
                OnCreateTime?.Invoke();
                yield return null;
            }
        }
    }
    public void EndPhase()
    {
        isEndPhase = true;
    }
    private void SetupDic(List<PhaseSettingData> phaseSettings)
    {
        phaseToSettingDic.Clear();
        phaseToIndexDict.Clear();
        phaseHitCountDict.Clear();
        for (int i = 0; i < phaseSettings.Count; i++)
        {
            var key = phaseSettings[i];
            phaseToSettingDic[key.gamePhase] = key;
            phaseToIndexDict[key.gamePhase] = i;
            phaseHitCountDict[key.gamePhase] = 0;

        }
    }
    private void OnHitHandle()
    {
        phaseHitCountDict[nowGamePhase]++;
        phaseChangeTime++;
    }
    [OnInspectorButton]
    public void ChangePhase(PhaseState nextPhase)
    {
        phaseIndex = phaseToIndexDict[nextPhase];
        EndPhase();
    }

}
