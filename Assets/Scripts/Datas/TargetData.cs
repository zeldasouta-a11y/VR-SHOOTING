using System;
using UnityEngine;


public enum MoveType { LinerMove , PendulumMove, UFOMove};
[Serializable]
public class TargetData
{
    [Header("Model Setting")]
    [SerializeField] private GameObject targetModel;
    public GameObject TargetModel => targetModel;

    [SerializeField] private string targetModelName;
    public string ModelName => targetModelName;
    [Header("Create Position Setting")]
    [SerializeField] Vector3 minPosition;
    public Vector3 MinPosition => minPosition;

    [SerializeField] Vector3 maxPosition;
    public Vector3 MaxPosition => maxPosition;

    [Header("出現上限数(MaxSpawnモード)"),Tooltip("出現上限数をいれてください")]
    [SerializeField] int maxSpawn = 100;
    public int MaxSpawn => maxSpawn;

    [Header("出現率の重み(SpawnWeightモード)"),Tooltip("出現率の重みをいれてください")]
    [SerializeField] int spawnWeight = 1;
    public int SpawnWeight => spawnWeight;

    [SerializeField] private int hitScore;
    public int HitScore => hitScore;

    [SerializeField] private bool hasVanishTime = false;
    public bool HasVanishTime => hasVanishTime;
    [EnableIf("hasVanishTime", hideWhenFalse: false)]
    [SerializeField] private float vanishTime;
    public float VanishTime => vanishTime;

    
    [SerializeField] private bool isMovable = false;
    public bool IsMovable => isMovable;
    [EnableIf("isMovable", hideWhenFalse: true)]
    [SerializeField] MoveType moveType;
    public MoveType MoveType => moveType;

    [EnableIfEnum("moveType", hideWhenFalse: true,MoveType.PendulumMove,MoveType.UFOMove)]
    [SerializeField] private float moveDurtation;
    public float MoveDurtation => moveDurtation;

    [EnableIf("isMovable", hideWhenFalse: true)]
    [SerializeField] private Vector3 moveVector;
    public Vector3 MoveVector => moveVector;

    public Vector3 GetRandomPosition()
    {
        return new Vector3(
            UnityEngine.Random.Range(MinPosition.x, MaxPosition.x),
            UnityEngine.Random.Range(MinPosition.y, MaxPosition.y),
            UnityEngine.Random.Range(MinPosition.z, MaxPosition.z)
        );
    }
}
