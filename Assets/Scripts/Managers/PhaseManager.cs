using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;

public class PhaseManager : MonoBehaviour
{

    [SerializeField] float phaseChangeTime = 3.0f;
    [SerializeField] float fixedUpdate = 0.1f;
    [EnableIf("isEndPhase",hideWhenFalse:false)]
    [SerializeField] PhaseState nowGamePhase;
    [SerializeField] bool phaseEndTrigger = true;
    [SerializeField] bool createTrigger = false;
    [SerializeField] private List<PhaseSettingData> phaseSettings;
    private Dictionary<PhaseState,PhaseSettingData> phaseToSettingDic = new();
    private Dictionary<PhaseState,int> phaseToIndexDict = new();
    private Dictionary<PhaseState,int> phaseHitCountDict = new();
    private Dictionary<PhaseState,Queue<int>> customIndexqueue = new(); 
    public Queue<int> CustomIndexQueue => customIndexqueue[Phase];
    RandomTable queueTable;
    //Resolve GC(Overhead)
    private static WaitForSeconds waitUpdate ;
    private static WaitForSeconds waitPhaseChange;
    private int phaseHitCount = 0;
    private int phaseIndex = 0;
    private bool isTutrialSkip = false;
    private bool isEndPhase = false;
    public PhaseState Phase
    {
        get => nowGamePhase;
        private set
        {
            nowGamePhase = value;
            //Event trigger
            if (phaseToSettingDic.TryGetValue(nowGamePhase, out var setting))
            {
                OnPhaseChanged?.Invoke(setting);
                Debug.Log($"NextMode:{value.ToString()}");
            }
            else
            {
                Debug.LogWarning($"[PhaseManager] Undefined phase: {nowGamePhase}");
            }
        }
    }
    public event Action<PhaseSettingData> OnPhaseChanged;
    public event Action OnCreateTime;
    public event Action OnPhaseEnd;
    public void SetEvent(GameManager gameManager)
    {
        gameManager.OnGameStart += OnGameStartHandle;
        gameManager.OnHit += OnHitHandle;
        
    }
    private void Start()
    {
       
        waitPhaseChange = new WaitForSeconds(phaseChangeTime);
        waitUpdate = new WaitForSeconds(fixedUpdate);
        isTutrialSkip = (ManagerLocator.Instance.Game.Tutorial == Tutorial.Skip);
        
    }
    private void OnDisable()
    {
        var gameManager = ManagerLocator.Instance.Game;
        if(gameManager != null)
        {
            gameManager.OnGameStart -= OnGameStartHandle;
            gameManager.OnHit -= OnHitHandle;
        }
    }
    private void OnGameStartHandle()
    {
        SetupAll(phaseSettings);
        StartCoroutine(GamePhaseMainTimer());

    }
    private IEnumerator GamePhaseMainTimer()
    {
        for (phaseIndex = 0; phaseIndex < phaseSettings.Count; phaseIndex++)
        {
            var phase = phaseSettings[phaseIndex];
            if (phase.gamePhase == PhaseState.Tutorial && isTutrialSkip)
            {
                continue;
            }
            isEndPhase = false;
            EndTriggerSet(false);
            Phase = phase.gamePhase;
            yield return waitPhaseChange;//先に待つ
            StartCoroutine(StartSpawn(phase));
            switch (phase.exitType)
            {
                case PhaseExitType.Trigger:
                    yield return RunWaitForTrigger();
                    break;
                case PhaseExitType.Time:
                    yield return RunPhase(phase.phaseTime);
                    break;
                case PhaseExitType.BrekeCount:
                    yield return RunWaitForBreak(phase.exitBreakCount);
                    break;
            }
            //フェイズ終了処理
            OnPhaseEnd?.Invoke();
            phaseHitCount = 0;
            isEndPhase = true;
        }
        ManagerLocator.Instance.Game.GameEnd();
    }
    private IEnumerator RunPhase(float phaseTime)
    {
        //StartCoroutine(CreateTimer(phase));
        for (float limitTimer = phaseTime; limitTimer >= 0; limitTimer -= fixedUpdate)
        {
            if (phaseEndTrigger) yield break;
            ManagerLocator.Instance.Game.UpdateUI(limitTimer);
            yield return waitUpdate;
        }
        
        ManagerLocator.Instance.Game.UpdateUI(0.0f);
    }
    private IEnumerator RunWaitForTrigger()
    {
        
        //WaitUntil() Falseの間待機
        //WaitWhile() Trueの間待機
        yield return new WaitWhile(() => !phaseEndTrigger);
    }
    private IEnumerator RunWaitForBreak(int count)
    {
        yield return new WaitWhile(() => phaseHitCount < count);
    }
    private IEnumerator StartSpawn(PhaseSettingData phase)
    {
        yield return null;//nullを回避するため待つ
        switch (phase.spawnTiming)
        {
            case SpawnTimingType.Time:
                yield return CreateByTime(phase);
                break;
            case SpawnTimingType.Count:
                yield return CreateByCount(phase.onSpawnCreateCount);
                break;
            case SpawnTimingType.Trigger:
                CreateTrigger(true);
                yield return CreateByTrigger(phase.onSpawnCreateCount);
                break;

        }
    }
    private IEnumerator CreateByTime(PhaseSettingData phase)
    {
        float totalTime = 0.0f;
        for (float timer = phase.createDuration; totalTime <= phase.phaseTime; timer += fixedUpdate, totalTime += fixedUpdate)
        {
            if (timer >= phase.createDuration)
            {
                if (isEndPhase) yield break;
                for (int i = 0; i < phase.onSpawnCreateCount; i++)
                {
                    OnCreateTime?.Invoke();
                    yield return null;
                }
                timer = 0.0f;
            }
            yield return waitUpdate;
        }
    }
    private IEnumerator CreateByCount(int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            OnCreateTime?.Invoke();
            yield return null;
        }
    }
    private IEnumerator CreateByTrigger(int spawnCount)
    {
        while (!isEndPhase)
        {
            yield return new WaitUntil(() => createTrigger || isEndPhase);//ここで待つことで余計なコードが走らない.
            if (isEndPhase) yield break;

            for (int i = 0; i < spawnCount; i++)
            {
                OnCreateTime?.Invoke();
                yield return null;
            }
            CreateTrigger(false);
        }
    }
    public void EndTriggerSet(bool trigger) => phaseEndTrigger = trigger;
    private void CreateTrigger(bool trigger) => createTrigger = trigger;
    private void SetupAll(List<PhaseSettingData> phaseSettings)
    {
        queueTable = new RandomTable(ManagerLocator.Instance.Game.GameSeed);
        phaseToSettingDic.Clear();
        phaseToIndexDict.Clear();
        phaseHitCountDict.Clear();
        for (int i = 0; i < phaseSettings.Count; i++)
        {
            var key = phaseSettings[i];
            phaseToSettingDic[key.gamePhase] = key;
            phaseToIndexDict[key.gamePhase] = i;
            phaseHitCountDict[key.gamePhase] = 0;
            //CustomQueueの生成
            if (phaseSettings[i].spawnChoose == SpawnChooseType.MaxSpawn)
            {
                List<int> tmpList = new();
                for (int j = 0; j < key.targetSettingSO.targetSettingData.Count; j++)
                {
                    int maxSpawn = key.targetSettingSO.targetSettingData[j].MaxSpawn;
                    for (int k = 0; k < maxSpawn; k++)
                    {
                        tmpList.Add(j);
                    }
                }
                queueTable.ShuffleList(tmpList);
                customIndexqueue[key.gamePhase] = new Queue<int>(tmpList);
            }
            else if (phaseSettings[i].spawnChoose == SpawnChooseType.SpawnWeight)
            {
                customIndexqueue[key.gamePhase] = new Queue<int>();//初期化
                int maxSpawn = phaseSettings[i].onSpawnCreateCount * 10;//10回生成と見なす(マジックナンバー)
                int total = 0;//重み合計
                List<TargetData> list = phaseSettings[i].targetSettingSO.targetSettingData;
                for (int j = 0; j < list.Count; j++)
                {
                    total += list[j].SpawnWeight;
                }
                for (int j = 0; j < maxSpawn; j++)
                {
                    int r = queueTable.RangeInt(0, total);//0~totalを生成
                    int value = 0;
                    for (int k = 0; k < list.Count; k++)
                    {
                        r -= list[j].SpawnWeight;//それぞれの重みで引き、初めて0以下になれば使用
                        if (r < 0)
                        {
                            value = j;
                            break;
                        }
                    }
                    customIndexqueue[key.gamePhase].Enqueue(value);
                }
            }
            else
            {
                customIndexqueue[key.gamePhase] = new();
            }
        }
    }
    private void OnHitHandle()
    {
        phaseHitCountDict[nowGamePhase]++;
        phaseHitCount++;
        if (phaseHitCount % phaseToSettingDic[Phase].onSpawnCreateCount == 0)
        {
            CreateTrigger(true);
        }
    }
    [OnInspectorButton]
    public void ChangePhase(PhaseState nextPhase)
    {
        phaseIndex = phaseToIndexDict[nextPhase]-1;
        EndTriggerSet(true);
    }

}
