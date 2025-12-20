using UnityEngine;

namespace VRShooting.Item
{
    public interface IUsable
    {
        bool IsExist{ get; }
        void Spawn();
        bool IsRespwanable{ get; }
        void Respawn();
    }

}
