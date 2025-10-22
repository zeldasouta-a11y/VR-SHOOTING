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
    [SerializeField] bool isTurorialPhase = true;
    [SerializeField] private List<PhaseSettingData> phaseSettings;
    private PhaseSettingData nowPhaseSetting;
    private Dictionary<PhaseState,PhaseSettingData> PhaseToSettingDic = new Dictionary<PhaseState, PhaseSettingData>();
    //Resolve GC(Overhead)
    private static WaitForSeconds waitUpdate ;
    private static WaitForSeconds waitPhaseChange;
    private PhaseState Phase
    {
        get => nowGamePhase;
        set
        {
            nowGamePhase = value;
            //Event trigger
            if (PhaseToSettingDic.TryGetValue(nowGamePhase, out var setting))
            {
                OnGamePhaseChanged?.Invoke(setting);
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
    public void SetEvent(GameManager gameManager)
    {
        gameManager.OnGameStart += OnGameStartHandle;
        OnGamePhaseChanged += OnPhaseChanedHandle;
    }
    private void Start()
    {
       
        waitPhaseChange = new WaitForSeconds(phaseChangeTime);
        waitUpdate = new WaitForSeconds(fixedUpdate);
        isTurorialPhase = (ManagerLocator.Instance.Game.Tutorial == Tutorial.Use);
        
    }
    private void OnDisable()
    {
        if(ManagerLocator.Instance.Game != null)
        {
            ManagerLocator.Instance.Game.OnGameStart -= OnGameStartHandle;
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
        foreach (PhaseSettingData phase in phaseSettings)
        {
            
            Phase = phase.gamePhase;
            StartCoroutine(CreateTimer(phase));
            if (!phase.hasExitTime)
            {
                yield return RunPhaseForWait();
            }
            else
            {
                yield return RunPhase(phase);
            }

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
    private IEnumerator RunPhaseForWait()
    {
        
        //WaitUntil() Falseの間待機
        //WaitWhile() Trueの間待機
        yield return new WaitWhile(() => isTurorialPhase);
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
    public void EndTutorial()
    {
        isTurorialPhase = false;
    }
    private void SetupDic(List<PhaseSettingData> phaseSettings)
    {
        PhaseToSettingDic.Clear();
        foreach (PhaseSettingData key in phaseSettings)
        {
            PhaseToSettingDic[key.gamePhase] = key;
        }
    }
    [OnInspectorButton]
    public void ChangePhase(PhaseState nextPhase)
    {
       Phase = nextPhase;
    }

}
