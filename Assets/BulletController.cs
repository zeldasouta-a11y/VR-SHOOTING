using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    /// <summary>
    /// 弾の速度 (m/s)
    /// </summary>
    [SerializeField]
    private float m_bulletSpeed = 27.0f;
    [Header("球を消す時間")]
    [SerializeField]
    private float m_destoryTime = 10.0f;
    private float time = 0;

    void Start()
    {
        time = m_destoryTime;
    }
    // Update is called once per frame
    void Update()
    {
        //弾を前に進ませる
        transform.position +=
            transform.forward * m_bulletSpeed * Time.deltaTime;

        time -= Time.deltaTime;
        if (time < 0){
            Destroy(gameObject);
        }
    }
}