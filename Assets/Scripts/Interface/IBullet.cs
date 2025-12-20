using UnityEngine;
using VRShooting.Player;

namespace VRShooting.Bullet
{
    public interface IBullet
    {
        public abstract void SetOwner(IScoreCollector owner);
        public abstract void BulletHit();
    }
}

