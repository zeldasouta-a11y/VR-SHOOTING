using UnityEngine;


namespace VRShooting
{
    [RequireComponent(typeof(Collider))]
    public class FogSetting : MonoBehaviour
    {
        [Range(0f,1f)]
        [SerializeField] float density = 0.05f;
        void OnCollisionEnter(Collision collision)
        {
            RenderSettings.fogDensity = density;
        }
        private void OnTriggerEnter(Collider other)
        {
            RenderSettings.fogDensity = density;
        }
    }
}
