using System;
using UnityEngine;

[Serializable]
public class TargetData 
{
    [SerializeField] public GameObject targetModel;
    [SerializeField] public  int hitScore;

    [SerializeField] public bool isVanish = false;
    [EnableIf("isVanish", hideWhenFalse: true)]
    [SerializeField] public float vanishTime;

    [SerializeField] public bool isMovable = false;

    [EnableIf(new string[] { "isMovable" , "!isPendulumMove" },ConditionLogic.AND,hideWhenFalse: true)]
    [SerializeField] public bool isUFOMove = false;

    [EnableIf(new string[] { "isMovable" , "!isUFOMove" },ConditionLogic.AND,hideWhenFalse: true)]
    [SerializeField] public bool isPendulumMove = false;

    [EnableIf(new string[] { "isPendulumMove" ,"isUFOMove"},ConditionLogic.OR,hideWhenFalse:true)]
    [SerializeField] public float moveDurtation;

    [EnableIf("isMovable", hideWhenFalse: true)]
    [SerializeField] public Vector3 moveVector;
}
