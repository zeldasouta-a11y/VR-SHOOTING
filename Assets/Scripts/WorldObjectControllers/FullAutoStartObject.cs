using UnityEngine;
using VRShooting.Bullet;
using VRShooting.Manager;

namespace VRShooting.Target
{
    public class FullAutoStartObject : MonoBehaviour,IHitReceiver
    {
        public void OnHitNotify(IHitSender hitsource)
        {
            var gameManager = ManagerLocator.Instance.Game;
            if (gameManager != null)
            {
                gameManager.StartFullAuto();
            }
        }
    }

}
