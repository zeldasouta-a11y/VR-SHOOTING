using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRShooting.Data;
namespace VRShooting.Manager
{
    public class PhaseManager : MonoBehaviour
    {

        [SerializeField] float phaseChangeTime = 3.0f;
        public float PhaseChangeTime => phaseChangeTime;
        [SerializeField] float fixedUpdate = 0.1f;
        [EnableIf("isEndPhase", hideWhenFalse: false)]
        [SerializeField] PhaseState nowGamePhase;
        public PhaseState Phase => nowGamePhase;
        public bool IsIgnoreScoreMode => phaseToSettingDic[Phase].isIgnoreScore;
        [SerializeField] bool isTimeOnlySpawn = false;
        [SerializeField] bool phaseEndTrigger = true;
        [SerializeField] int createTriggerCount = 0;
        [SerializeField] private List<PhaseSettingData> phaseSettings;
        private Dictionary<PhaseState, PhaseSettingData> phaseToSettingDic = new();
        private Dictionary<PhaseState, int> phaseToIndexDict = new();
        private Dictionary<PhaseState, int> phaseHitCountDict = new();
        private Dictionary<PhaseState, Queue<int>> customIndexqueue = new();
        private string[] dotArray = new string[4] { "", ".", "..", "..." };
        public Queue<int> CustomIndexQueue => customIndexqueue[Phase];
        RandomTable queueTable;
        //Resolve GC(Overhead)
        private static WaitForSeconds waitUpdate;
        private static WaitForSeconds wait05s = new WaitForSeconds(0.5f);
        private int phaseHitCount = 0;
        private int phaseIndex = 0;
        private bool isTutrialSkip = false;
        private bool isEndPhase = false;
        private Coroutine _spawnCorutine;
        private Coroutine gameMainCorutine;

        public event Action<PhaseSettingData> OnPhaseChanged;
        public event Action OnCreateTime;
        public event Action OnPhaseEnd;
        public event Action OnAllPhaseEnd;
        public void SetEvent(GameManager gameManager)
        {
            gameManager.OnGameStart += OnGameStartHandle;
            gameManager.OnHit += OnHitHandle;
            gameManager.OnGameEnd += OnGameEndHandle;

        }
        private void Start()
        {
            waitUpdate = new WaitForSeconds(fixedUpdate);
        }
        private void OnDisable()
        {
            var gameManager = ManagerLocator.Instance.Game;
            if (gameManager != null)
            {
                gameManager.OnGameStart -= OnGameStartHandle;
                gameManager.OnHit -= OnHitHandle;
                gameManager.OnGameEnd -= OnGameEndHandle;
            }
        }

        private void OnGameEndHandle(bool isGameComplete)
        {
            if (!isGameComplete && gameMainCorutine != null)
            {
                OnPhaseEnd?.Invoke();
                StopAllCoroutines();
            }
        }

        private void PhaseChange(PhaseState newPhase)
        {
            nowGamePhase = newPhase;
            //Event trigger
            if (phaseToSettingDic.TryGetValue(nowGamePhase, out var setting))
            {
                OnPhaseChanged?.Invoke(setting);
            }
            else
            {
                Debug.LogWarning($"[PhaseManager] Undefined phase: {nowGamePhase}");
            }
        }
        private void OnGameStartHandle()
        {
            isTutrialSkip = (ManagerLocator.Instance.Game.Tutorial == Tutorial.Skip);
            SetupAll(phaseSettings);
            gameMainCorutine = StartCoroutine(GamePhaseMainTimer());

        }
        private IEnumerator GamePhaseMainTimer()
        {
            for (phaseIndex = 0; phaseIndex < phaseSettings.Count; phaseIndex++)
            {
                var phase = phaseSettings[phaseIndex];
                //チュートリアルスキップの場合
                if (isTutrialSkip && (phase.gamePhase == PhaseState.Tutorial || phase.gamePhase == PhaseState.TitorialBoard))
                {
                    continue;
                }
                isEndPhase = false;
                EndTriggerSet(false);

                PhaseChange(phase.gamePhase);
                if (!phase.isInstantlyChange)
                {
                    var UI = ManagerLocator.Instance.UI;
                    int dotCount = 0;
                    //先に待つ
                    for (float i = 0; i <= phaseChangeTime; i += 0.5f)
                    {
                        dotCount %= dotArray.Length;
                        UI.UpdateTimerUI("loading" + dotArray[dotCount++]);
                        yield return wait05s;
                    }
                    UI.UpdateTimerUI("");
                }
                _spawnCorutine = StartCoroutine(StartSpawn(phase));
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
                //CreateByTrigger()待つ処理が終わらない ので止める
                //バグだけど、仕様として残す。
                if (isTimeOnlySpawn)
                {
                    StopCorutine();
                }
                isEndPhase = true;
            }
            OnAllPhaseEnd?.Invoke();
        }
        private IEnumerator RunPhase(float phaseTime)
        {
            //StartCoroutine(CreateTimer(phase));
            for (float limitTimer = phaseTime; limitTimer >= 0; limitTimer -= fixedUpdate)
            {
                if (phaseEndTrigger) yield break;
                ManagerLocator.Instance.UI.UpdateTimerUI(limitTimer);
                yield return waitUpdate;
            }

            ManagerLocator.Instance.UI.UpdateTimerUI(0.0f);
        }
        private IEnumerator RunWaitForTrigger()
        {

            //WaitUntil() Falseの間待機
            //WaitWhile() Trueの間待機
            yield return new WaitWhile(() => !phaseEndTrigger);
        }
        private IEnumerator RunWaitForBreak(int count)
        {
            string text = $"remain:{phaseToSettingDic[nowGamePhase].exitBreakCount}";
            ManagerLocator.Instance.UI.UpdateTimerUI(text);
            yield return new WaitWhile(() => phaseHitCount < count);
            ManagerLocator.Instance.UI.UpdateTimerUI("Tutorial Clear!");
        }
        private IEnumerator StartSpawn(PhaseSettingData phase)
        {
            yield return null;//nullを回避するため待つ
            switch (phase.spawnTiming)
            {
                case SpawnTimingType.Time:
                    yield return CreateByTime(phase);
                    yield break;
                case SpawnTimingType.Count:
                    yield return CreateByCount(phase.onSpawnTimeCount);
                    yield break;
                case SpawnTimingType.Trigger:
                    CreateTriggerCount(1);
                    yield return CreateByTrigger(phase.onSpawnTriggerCount);
                    yield break;
            }
        }
        private void StopCorutine()
        {
            if (_spawnCorutine != null)
            {
                StopCoroutine(_spawnCorutine);
                _spawnCorutine = null;
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
                    for (int i = 0; i < phase.onSpawnTimeCount; i++)
                    {
                        //Debug.Log("Create By Time");
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
                //Debug.Log("Create By Count");
                OnCreateTime?.Invoke();
                yield return null;
            }
        }

        //ここで詰まってる
        //
        private IEnumerator CreateByTrigger(int spawnCount)
        {
            while (!isEndPhase)
            {
                yield return new WaitUntil(() => createTriggerCount > 0 || isEndPhase);//ここで待つことで余計なコードが走らない.
                if (isEndPhase) yield break;

                for (int i = 0; i < spawnCount; i++)
                {
                    Debug.Log("Create By Trigger");
                    OnCreateTime?.Invoke();
                    yield return null;
                }
                CreateTriggerCount(-1);
            }
        }
        public void EndTriggerSet(bool trigger) => phaseEndTrigger = trigger;
        private void CreateTriggerCount(int cnt) => createTriggerCount += cnt;
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
                    int maxSpawn = phaseSettings[i].onSpawnTimeCount * 10;//10回生成と見なす(マジックナンバー)
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
            if (phaseToSettingDic[nowGamePhase].exitType == PhaseExitType.BrekeCount)
            {
                string text = "remain:";
                text += phaseToSettingDic[nowGamePhase].exitBreakCount - phaseHitCount;
                ManagerLocator.Instance.UI.UpdateTimerUI(text);
            }
            //一定数出現する敵を打つと次が出る
            if (phaseToSettingDic[Phase].onSpawnTimeCount != 0 && phaseHitCount % phaseToSettingDic[Phase].onSpawnTimeCount == 0)
            {
                CreateTriggerCount(1);
            }
        }
        [OnInspectorButton]
        public void ChangePhase(PhaseState nextPhase)
        {
            phaseIndex = phaseToIndexDict[nextPhase] - 1;
            EndTriggerSet(true);
        }

    }

}
