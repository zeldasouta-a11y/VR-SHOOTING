using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "TargetSetting")]

public class TargetDataSO : ScriptableObject
{
    public List<TargetData> targetSettingData;
}
