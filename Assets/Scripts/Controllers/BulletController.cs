using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour,IHitSender
{
    
    BulletData bulletdata;

    private void OnEnable()
    {
        StartCoroutine(ReturnToPoolAfterDelay());
    }
    // Update is called once per frame
    void Update()
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

    public void OnHit()
    {}
}