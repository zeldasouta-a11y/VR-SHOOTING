using UnityEngine;
using VRShooting.Bullet;
using VRShooting.Manager;

namespace VRShooting.Target
{
    public class NextPhaseObject : MonoBehaviour,IHitReceiver
    {
        public void OnHitNotify(IHitSender hitsource)
        {
            ManagerLocator.Instance.Phase.EndTriggerSet(true);
        }

    }

}
