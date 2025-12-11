using UnityEngine;
using VRShooting.Player;
using VRShooting.Target;

namespace VRShooting.Bullet
{
    /// <summary>
    /// 弾などヒットさせるもの
    /// </summary>
    public interface IHitSender
    {
        public abstract IScoreCollector GetScoreCollector{get;}
        public abstract void OnHit(IHitReceiver receiver);
    }
}

