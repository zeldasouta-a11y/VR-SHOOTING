using UnityEngine;

namespace VRShooting.Bullet
{
    /// <summary>
    /// 弾などヒットさせるもの
    /// </summary>
    public interface IHitSender
    {
        public abstract void OnHit();
    }
}

