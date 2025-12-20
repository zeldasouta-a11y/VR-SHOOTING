using UnityEngine;
using VRShooting.Player;

namespace VRShooting.Item
{
    public interface IWeapon : IUsable
    {
        public abstract void SetOwner(IScoreCollector owner);
        public abstract void WeaponShot();
    }
}
