using UnityEngine;
using VRShooting.Bullet;
using VRShooting.Target;

namespace VRShooting.Item
{
    public class SpawnItem : MonoBehaviour,IHitReceiver
    {
        [SerializeField] GameObject spawnObject;

        public void OnHitNotify(IHitSender hitsource)
        {
            if (spawnObject == null) return;
            GameObject obj = Instantiate(spawnObject);
            IUsable usable = obj.GetComponent<IUsable>();
            if (usable == null) return;
            usable.Spawn();
        }
    }

}
