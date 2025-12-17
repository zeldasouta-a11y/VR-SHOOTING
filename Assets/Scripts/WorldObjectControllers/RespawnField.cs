using UnityEngine;
using VRShooting.Player;
using VRShooting.Weapon;

namespace VRShooting.Filed
{
    public class RespawnField : MonoBehaviour
    {
        void OnTriggerEnter(Collider collision)
        {
            PlayerContoller contoller = collision.gameObject.GetComponent<PlayerContoller>();
            if (contoller != null)
            {
                contoller.PlayerRespawn();
            }
            GunController gunController= collision.gameObject.GetComponentInParent<GunController>();
            if(gunController != null)
            {
                gunController.GunRespawn();
            }
#if UNITY_EDITOR
            Debug.Log($"[RespawnField] {collision.gameObject.name}");
#endif
        }
    }

}
