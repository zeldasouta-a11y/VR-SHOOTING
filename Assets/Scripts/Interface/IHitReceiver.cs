using UnityEngine;
using VRShooting.Bullet;

namespace VRShooting.Target
{
    /// <summary>
    /// ヒットを受信するもの
    /// </summary>
    public interface IHitReceiver
    {
        public abstract void OnHitNotify(IHitSender hitsource);
    }

}
