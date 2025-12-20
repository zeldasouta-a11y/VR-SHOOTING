using System.Collections;
using UnityEngine;
using VRShooting.Player;
using VRShooting.Target;
namespace VRShooting.Bullet
{
    [RequireComponent(typeof(SphereCollider))]
    public class ExplodeWind : MonoBehaviour,IHitSender
    {
        SphereCollider sphereCollider;
        [SerializeField] float vanishTime = 0.1f;
        [SerializeField] float extendSize = 10;
        [SerializeField] float extendInitialzie = -10;
        float radius = 0;
        IScoreCollector collector;

        public IScoreCollector ScoreCollector => collector;

        public void OnHit(IHitReceiver receiver)
        {
            receiver.OnHitNotify(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            StartCoroutine(Expload(vanishTime));
        }
        public void SetExpand(float expand)
        {
            extendSize = expand;
        }

        IEnumerator Expload(float vanishTime)
        {
            sphereCollider.radius = extendSize;
            yield return null;
            sphereCollider.radius = 0;
        }
        void OnCollisionEnter(Collision collision)
        {
            IHitReceiver receiver = collision.gameObject.GetComponent<IHitReceiver>();
            if(receiver != null)
            {
                OnHit(receiver);
            }
        }
        void OnTriggerEnter(Collider collision)
        {
            IHitReceiver receiver = collision.gameObject.GetComponent<IHitReceiver>();
            if(receiver != null)
            {
                OnHit(receiver);
            }
        }
    }
}

