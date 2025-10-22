using System;
using UnityEngine;

public enum PhaseState { Tutorial, Phase1, Phase2, Phase3, Phase4, Phase5 }
public enum SpawnType { Time, Count };
[Serializable]
public class PhaseSettingData
{

    public PhaseState gamePhase;
    public float phaseTime;
    public bool hasExitTime;
    [Header("Target Setting")]
    public SpawnType spawnType;
    public float createDuration;
    public int onSpawnCreateCount;
    public TargetDataSO targetSettingSO;
    //[Header("Gun Setting")]
    //public int fireRate;
    //public int ReloadConstant;

}