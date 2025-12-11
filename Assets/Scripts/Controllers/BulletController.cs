using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRShooting.Data;
using VRShooting.Manager;
namespace VRShooting.Bullet
{
    /// <summary>
    /// 弾の基底
    /// </summary>
    public class BulletController : MonoBehaviour, IHitSender,IBullet
    {

        BulletData bulletdata;

        protected virtual void OnEnable()
        {
            StartCoroutine(ReturnToPoolAfterDelay());
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            //弾を前に進ませる
            transform.position +=
                transform.forward * bulletdata.BulletSpeed * Time.deltaTime;
        }
        public void Init(BulletData data)
        {
            bulletdata = data;
        }
        private IEnumerator ReturnToPoolAfterDelay()
        {
            yield return null;//1f待つことで参照切れを防ぐ.
            yield return new WaitForSeconds(bulletdata.BulletVanishTime);
            ManagerLocator.Instance.Bullet.ReturnBullet(bulletdata.Type, this.gameObject);
        }

        public  virtual void OnHit()
        { }

        public virtual void BulletHit()
        {
            
        }
    }
}
