using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRShooting.Player;
using VRShooting.Target;
using VRShooting.Item;
namespace VRShooting.Bullet
{
    public class ProjectileController : BulletController
    {
        // --- Config ---
        public float speed = 500;
        private float elapsed = 0;
        public LayerMask collisionLayerMask;

        // --- Explosion VFX ---
        public GameObject rocketExplosion;

        // --- Projectile Mesh ---
        public MeshRenderer projectileMesh;

        // --- Script Variables ---
        private bool targetHit;

        // --- Audio ---
        public AudioSource inFlightAudioSource;

        // --- VFX ---
        public ParticleSystem disableOnHit;
        
        [Header("Explosion expand (distance -> expand)")]
        [SerializeField] private float expandMin = 2f;          // 近距離の爆発範囲
        [SerializeField] private float expandMax = 10f;         // 遠距離の上限
        [SerializeField] private float expandStartDistance = 0f; // ここまではほぼ最小
        [SerializeField] private float saturationDistance = 50f; // startからこの距離でほぼ上限へ
        [SerializeField, Range(0.5f, 0.999f)] private float saturationPercent = 0.95f;


        protected override void Update()
        {
            // --- Check to see if the target has been hit. We don't want to update the position if the target was hit ---
            if (targetHit) return;

            // --- moves the game object in the forward direction at the defined speed ---
            float move = (speed * Time.deltaTime);
            transform.position += transform.forward * move;
            elapsed += move;
        }


        /// <summary>
        /// Explodes on contact.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            IWeapon weapon = collision.gameObject.GetComponentInParent<IWeapon>();
            if(weapon != null) return;
            PlayerContoller player = collision.gameObject.GetComponentInParent<PlayerContoller>();
            if(player != null) return;
            
            IHitSender sender = collision.gameObject.GetComponentInParent<IHitSender>();
            if(sender != null) return;
            Terrain terrain = collision.gameObject.GetComponentInParent<Terrain>();
            if(terrain != null) return;
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (!enabled) return;
            Debug.Log($"[ProjectileController] {collision.gameObject.name} Collision Hit");
            // --- Explode when hitting an object and disable the projectile mesh ---
            Explode();
            projectileMesh.enabled = false;
            targetHit = true;
            inFlightAudioSource.Stop();
            foreach(Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            disableOnHit.Stop();


            // --- Destroy this object after 2 seconds. Using a delay because the particle system needs to finish ---
            Destroy(gameObject, 5f);
        }
        /// <summary>
        /// Explodes on contact.
        /// </summary>
        /// <param name="collision"></param>
        private float CalcExpand(float distance)
        {
            float x = Mathf.Max(0f, distance - expandStartDistance);

            // 「saturationDistance で saturationPercent に到達」する k を計算
            float p = Mathf.Clamp(saturationPercent, 0.0001f, 0.9999f);
            float k = -Mathf.Log(1f - p) / Mathf.Max(0.0001f, saturationDistance);

            float t = 1f - Mathf.Exp(-k * x); // 0→1へ指数的に近づく
            return Mathf.Lerp(expandMin, expandMax, t);
        }

        void OnTriggerEnter(Collider collision)
        {
            IWeapon weapon = collision.gameObject.GetComponentInParent<IWeapon>();
            if(weapon != null) return;
            PlayerContoller player = collision.gameObject.GetComponentInParent<PlayerContoller>();
            if(player != null) return;
            
            IHitSender sender = collision.gameObject.GetComponentInParent<IHitSender>();
            if(sender != null) return;
            Terrain terrain = collision.gameObject.GetComponentInParent<Terrain>();
            if(terrain != null) return;
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (!enabled) return;
            Debug.Log($"[ProjectileController] {collision.gameObject.name} Trigger Hit");
            // --- Explode when hitting an object and disable the projectile mesh ---
            Explode();
            projectileMesh.enabled = false;
            targetHit = true;
            inFlightAudioSource.Stop();
            foreach(Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            disableOnHit.Stop();


            // --- Destroy this object after 2 seconds. Using a delay because the particle system needs to finish ---
            Destroy(gameObject, 5f);
        }


        /// <summary>
        /// Instantiates an explode object.
        /// </summary>
        private void Explode()
        {
            // --- Instantiate new explosion option. I would recommend using an object pool ---
            GameObject newExplosion = Instantiate(rocketExplosion, transform.position, rocketExplosion.transform.rotation, null);
            float expand = CalcExpand(elapsed);
             // 見た目の拡大（VFX全体を拡大）
            newExplosion.transform.localScale = Vector3.one * expand;
            ExplodeWind wind = newExplosion.GetComponent<ExplodeWind>();
            wind.SetExpand(expand);
            Vector3 baseScale = rocketExplosion.transform.localScale;
            newExplosion.transform.localScale = baseScale * expand;
        }
        public override void BulletHit()
        {
            Explode();
        }
    }
}