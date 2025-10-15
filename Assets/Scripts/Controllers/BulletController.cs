using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    
    [SerializeField] BulletData bulletdata;

    void Start()
    {
        Destroy(gameObject, bulletdata.BulletVanishTime);
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
}