using System;
using UnityEngine;

public enum PhaseState { Start, Tutorial, Easy, Normal, Hard, AttashCase, Bonus }
public enum PhaseExitType { Time,Trigger ,BrekeCount}
public enum SpawnTimingType { Time, Count, Trigger };
[Serializable]
public class PhaseSettingData
{
    [Header("Phase Name")]
    public PhaseState gamePhase;
    [Header("To Next Phase Setting")]
    public PhaseExitType exitType;
    [EnableIfEnum("exitType",hideWhenFalse :true,PhaseExitType.Time)]
    public float phaseTime = 0;
    [EnableIfEnum("exitType", hideWhenFalse: true, PhaseExitType.BrekeCount)]
    public int exitBreakCount = 0;
    [Header("Target Setting")]
    public SpawnTimingType spawnTiming;
    [EnableIfEnum("spawnTiming",hideWhenFalse:true,SpawnTimingType.Time)]
    public float createDuration = 0;
    public int onSpawnCreateCount = 0;
    public SpawnChooseType spawnChoose;
    public TargetDataSO targetSettingSO;
    //[Header("Gun Setting")]
    //public int fireRate;
    //public int ReloadConstant;

}