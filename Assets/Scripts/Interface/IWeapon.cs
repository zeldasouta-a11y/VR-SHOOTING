using System;
using UnityEngine;

namespace VRShooting.Item
{
    public interface IWeapon : IUsable
    {
        public abstract void WeaponShot();
    }
}
