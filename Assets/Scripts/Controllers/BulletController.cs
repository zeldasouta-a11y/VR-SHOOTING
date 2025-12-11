using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRShooting.Data;
using VRShooting.Manager;
using VRShooting.Player;
using VRShooting.Target;
namespace VRShooting.Bullet
{
    /// <summary>
    /// 弾の基底
    /// </summary>
    public class BulletController : MonoBehaviour, IHitSender,IBullet
    {

        BulletData bulletdata;
        IScoreCollector collector;
        private float bulletSpeed = 0;
        private float bulletVanishTime = 3.0f;
        private BulletType bulletType;

        public IScoreCollector GetScoreCollector => collector;

        protected virtual void OnEnable()
        {
            StartCoroutine(ReturnToPoolAfterDelay());
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            //弾を前に進ませる
            transform.position +=
                transform.forward * bulletSpeed * Time.deltaTime;
        }
        public void SetIScoreCollector(IScoreCollector collector)
        {
            this.collector = collector;
        }
        public void Init(BulletData data)
        {
            bulletdata = data;
            bulletSpeed = bulletdata.BulletSpeed;
            bulletVanishTime = bulletdata.BulletVanishTime;
            bulletType = bulletdata.Type;
        }
        private IEnumerator ReturnToPoolAfterDelay()
        {
            yield return null;//1f待つことで参照切れを防ぐ.
            yield return new WaitForSeconds(bulletVanishTime);
            ManagerLocator.Instance.Bullet.ReturnBullet(bulletType, this.gameObject);
        }

        public  virtual void OnHit(IHitReceiver receiver)
        { }

        public virtual void BulletHit()
        {
            
        }
    }
}
