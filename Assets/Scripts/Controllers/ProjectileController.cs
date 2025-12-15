using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRShooting.Player;
using VRShooting.Target;
using VRShooting.Weapon;
namespace VRShooting.Bullet
{
    public class ProjectileController : BulletController
    {
        // --- Config ---
        public float speed = 100;
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
            IWeapon weapon = collision.gameObject.GetComponent<IWeapon>();
            if(weapon != null) return;
            PlayerContoller player = collision.gameObject.GetComponentInParent<PlayerContoller>();
            if(player != null) return;
            
            IHitSender sender = collision.gameObject.GetComponentInParent<IHitSender>();
            if(sender != null) return;
            Terrain terrain = collision.gameObject.GetComponentInParent<Terrain>();
            if(terrain != null) return;
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (!enabled) return;
            Debug.Log(collision.gameObject.name + " Hit");
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
        void OnTriggerEnter(Collision collision)
        {
            IWeapon weapon = collision.gameObject.GetComponent<IWeapon>();
            if(weapon != null) return;
            PlayerContoller player = collision.gameObject.GetComponentInParent<PlayerContoller>();
            if(player != null) return;
            
            IHitSender sender = collision.gameObject.GetComponentInParent<IHitSender>();
            if(sender != null) return;
            Terrain terrain = collision.gameObject.GetComponentInParent<Terrain>();
            if(terrain != null) return;
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (!enabled) return;
            Debug.Log(collision.gameObject.name + " Hit");
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
            ExplodeWind wind = newExplosion.GetComponent<ExplodeWind>();
            wind.SetExpand(elapsed);



        }
        public override void BulletHit()
        {
            Explode();
        }
    }
}