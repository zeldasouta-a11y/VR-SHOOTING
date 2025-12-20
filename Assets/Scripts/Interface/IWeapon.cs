using UnityEngine;
using VRShooting.Player;

namespace VRShooting.Weapon
{
    public interface IWeapon
    {
        public abstract void SetOwner(IScoreCollector owner);
        public abstract void WeaponShot();
    }
}
