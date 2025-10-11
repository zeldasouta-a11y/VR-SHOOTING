using System;
using UnityEngine;

[Serializable]
public class TargetDeta 
{
    [SerializeField] public GameObject targetModel;
    [SerializeField] public  int hitScore;
    [SerializeField] public bool isVanish;
    [EnableIf("isVanish", hideWhenFalse: true)]
    [SerializeField] public float vanishTime;
}
