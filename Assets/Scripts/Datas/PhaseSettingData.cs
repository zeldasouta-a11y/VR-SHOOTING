using System;
using UnityEngine;

public enum PhaseState { Start, Tutorial, Easy, Normal, Hard, AttashCase, Bonus }
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