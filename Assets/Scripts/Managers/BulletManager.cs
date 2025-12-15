using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using VRShooting.Bullet;
using VRShooting.Data;
using VRShooting.Player;
namespace VRShooting.Manager
{
    public class BulletManager : MonoBehaviour
    {
        [Serializable]
        public class BulletInfo
        {
            public GameObject prefab;
            public BulletData data;
            public int initializeCount = 20;
        }
        //ここで弾丸データを管理するなら
        [SerializeField] List<BulletInfo> bulletInfoList = new List<BulletInfo>();
        //Object Pool
        Dictionary<BulletType, Queue<GameObject>> bulletPoolDict = new Dictionary<BulletType, Queue<GameObject>>();
        Dictionary<BulletType, BulletInfo> bulletInfoDict = new Dictionary<BulletType, BulletInfo>();

        private void Start()
        {
            foreach (var info in bulletInfoList)
            {
                Queue<GameObject> q = new();
                for (int i = 0; i < info.initializeCount; i++)
                {
                    var obj = Instantiate(info.prefab);
                    obj.SetActive(false);
                    BulletController bulletController = obj.GetComponent<BulletController>();
                    if(bulletController == null)
                    {
                        bulletController = obj.AddComponent<BulletController>();
                        UnityEngine.Debug.LogWarning($"{obj.name} has not BulletController. Please Attach BulletContorller ");
                    }
                    bulletController.Init(info.data);

                    q.Enqueue(obj);
                }
                bulletPoolDict.Add(info.data.Type, q);
                bulletInfoDict.Add(info.data.Type, info);
            }
        }

        public GameObject ActiveBullet(BulletType type, Vector3 pos, Quaternion rot,IScoreCollector collector)
        {
            if (!bulletPoolDict.ContainsKey(type)) return null;
            Queue<GameObject> bulletqueue = bulletPoolDict[type];
            GameObject obj = bulletqueue.Count > 0 ? bulletqueue.Dequeue() : CreateInsitatce(type);
            ////弾の位置を、銃口の位置と同一にする。
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.GetComponent<BulletController>().SetIScoreCollector(collector);
            obj.SetActive(true);
            return obj;
        }
        public void ReturnBullet(BulletType type, GameObject obj)
        {
            Rigidbody rbody = obj.GetComponent<Rigidbody>();
            rbody.linearVelocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;
            obj.SetActive(false);
            bulletPoolDict[type].Enqueue(obj);
        }

        private GameObject CreateInsitatce(BulletType type)
        {
            BulletInfo info = bulletInfoDict[type];

            GameObject clone = Instantiate(info.prefab);
            clone.SetActive(false);
            BulletController bulletController = clone.GetComponent<BulletController>();
            bulletController.Init(info.data);
            return clone;
        }
    }

}
