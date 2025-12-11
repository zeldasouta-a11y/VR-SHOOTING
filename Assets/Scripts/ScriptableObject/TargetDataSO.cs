using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace VRShooting.Data
{
    [CreateAssetMenu(menuName = "TargetSetting")]

    public class TargetDataSO : ScriptableObject
    {
        public List<TargetData> targetSettingData;
    }

}
