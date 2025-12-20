using UnityEngine;
using VRShooting.Bullet;
using VRShooting.Manager;
using VRShooting.Target;

namespace VRShooting.Item
{
    public class ChangeBGM : MonoBehaviour, IHitReceiver
    {
        public void OnHitNotify(IHitSender hitsource)
        {
            ManagerLocator.Instance.Game.UpdateBGM();
        }
    }

}
