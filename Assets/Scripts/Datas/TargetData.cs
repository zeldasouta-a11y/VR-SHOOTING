using System;
using UnityEngine;

[Serializable]
public class TargetData 
{
    [SerializeField] private GameObject targetModel;
    public GameObject TargetModel => targetModel;
    [Header("Create Position Setting")]
    [SerializeField] Vector3 minPosition;
    public Vector3 MinPosition => minPosition;

    [SerializeField] Vector3 maxPosition;
    public Vector3 MaxPosition => maxPosition;

    [SerializeField] private int hitScore;
    public int HitScore => hitScore;

    [SerializeField] private bool isVanish = false;
    public bool IsVanish => isVanish;
    [EnableIf("isVanish", hideWhenFalse: true)]
    [SerializeField] private float vanishTime;
    public float VanishTime => vanishTime;

    [SerializeField] private bool isMovable = false;
    public bool IsMovable => isMovable;

    [EnableIf(new string[] { "isMovable" , "!isPendulumMove" },ConditionLogic.AND,hideWhenFalse: true)]
    [SerializeField] private bool isUFOMove = false;
    public bool IsUFOMove => isUFOMove;

    [EnableIf(new string[] { "isMovable" , "!isUFOMove" },ConditionLogic.AND,hideWhenFalse: true)]
    [SerializeField] private bool isPendulumMove = false;
    public bool IsPendulumMove => isPendulumMove;

    [EnableIf(new string[] { "isPendulumMove" ,"isUFOMove"},ConditionLogic.OR,hideWhenFalse:true)]
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
    public void ModeChange(Vector3 newMinPos,Vector3 newMaxPos,Vector3 newMoveVec)
    {
        minPosition = newMinPos;
        maxPosition = newMaxPos;
        moveVector = newMoveVec;
    }
}
