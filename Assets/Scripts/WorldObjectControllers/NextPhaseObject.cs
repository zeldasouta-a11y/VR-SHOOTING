using UnityEngine;
using VRShooting.Manager;

namespace VRShooting.Target
{
    public class NextPhaseObject : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnDisable()
        {
            ManagerLocator.Instance.Phase.EndTriggerSet(true);
        }
    }

}
