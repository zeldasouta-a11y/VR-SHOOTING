using Unity.XR.CoreUtils;
using UnityEngine;
using VRShooting.Weapon;

namespace VRShooting.Player
{
    public class PlayerContoller : MonoBehaviour
    {
        public XROrigin origin { get; private set; }
        public IWeapon usingWapon { get; private set; }


        private void Start()
        {
            origin = GetComponent<XROrigin>();
        }
        [OnInspectorButton]
        public void PlayerRespawn()
        {
            if (this.gameObject.transform.localPosition.y < -1)
            {
                this.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            }
        }
        void FixedUpdate()
        {
            if (this.gameObject.transform.localPosition.y < -1)
            {
                PlayerRespawn();
            }
        }
    }
}

