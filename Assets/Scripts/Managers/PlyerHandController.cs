using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VRShooting.Player;
using VRShooting.Item;


namespace VRShooting.Pl
{
    public enum HandType { Left,Right}
    public class PlyerHandController : MonoBehaviour
    {
        [Header("Hands")]
        [SerializeField] private XRBaseInteractor leftHand;
        [SerializeField] private XRBaseInteractor rightHand;

        public GameObject LeftHeld { get; private set; }
        public GameObject RightHeld { get; private set; }
        public IScoreCollector owner { get; private set; }

        private void Start()
        {
            ResisrerHand(leftHand, HandType.Left);
            ResisrerHand(rightHand, HandType.Right);
            owner = GetComponent<IScoreCollector>();
            if(owner == null)
            {
                Debug.LogError("Player is not Found");
            }
        }

        private void ResisrerHand(XRBaseInteractor hand, HandType handtype)
        {
            hand.selectEntered.AddListener(args =>
            {
                var obj = args.interactableObject.transform.GetComponent<IWeapon>();
                if (obj == null) return;

                obj.SetOwner(owner);


            });
            hand.selectExited.AddListener(args =>
            {
                ClearHeldServerRpc(handtype);
            });
        }
        private void SetHoldObj(GameObject owner,HandType hand) {
            UpdateLocalHeld(owner, hand);   
        }

        private void ClearHeldServerRpc(HandType hand)
        {
            UpdateLocalHeld(null, hand);
        }
        private void UpdateLocalHeld(GameObject obj, HandType hand)
        {
            if (hand == HandType.Left)
                LeftHeld = obj;
            else
                RightHeld = obj;
        }
    }
}

