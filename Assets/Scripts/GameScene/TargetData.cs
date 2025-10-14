using System;
using UnityEngine;

[Serializable]
public class TargetData 
{
    [SerializeField] private GameObject targetModel;
    public GameObject TargetModel => targetModel;

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
}
