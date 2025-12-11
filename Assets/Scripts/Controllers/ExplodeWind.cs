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
        [SerializeField] float vanishTime = 3.0f;
        [SerializeField] float extendSize = 10;
        IScoreCollector collector;

        public IScoreCollector GetScoreCollector => collector;

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

        IEnumerator Expload(float vanishTime)
        {
            float time = 0;
            while (time < vanishTime)
            {
                time += Time.deltaTime;
                sphereCollider.radius += Time.deltaTime*extendSize;
                yield return null;
            }
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

