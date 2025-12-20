using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using VRShooting.Bullet;
using VRShooting.Data;
using VRShooting.Manager;
namespace VRShooting.Target
{
    [RequireComponent(typeof(Rigidbody))]
    public class TargetController : MonoBehaviour, IHitReceiver,IScorable
    {
        [SerializeField] GameObject pointCanvasObject;
        [SerializeField] Canvas canvas;
        [SerializeField] TextMeshProUGUI hittext;
        [SerializeField] TargetData targetDatas;
        private GameObject targetModel = null;
        private GameObject targetParentObject = null;
        private static WaitForSeconds waitThree = new WaitForSeconds(3.0f);
        private Vector3 inverseY = new Vector3(1, -1, 1);
        private Vector3 spawnPoint;
        private Vector3 moving;
        private HashSet<IHitReceiver> receivers = new();
        private readonly WaitForSeconds fixedUpdate = new WaitForSeconds(1f);
        private float time = 0f;
        private bool isEnabled = false;

        public int Score => targetDatas.HitScore;

        public string Name => targetDatas.ModelName;

        public System.Tuple<int, string> ScoreAndName => new(Score, Name);

        public void Init(TargetData data, GameObject model, Vector3 spawnAt, Camera targetCamera)
        {
            targetDatas = data;
            targetParentObject = model;
            spawnPoint = spawnAt;
            canvas.worldCamera = targetCamera;
            isEnabled = false;
            foreach (Transform child in model.transform)
            {
                if (child.gameObject.CompareTag("Model"))
                {
                    targetModel = child.gameObject;
                }
            }
            if (hittext == null)
            {
                hittext = pointCanvasObject.AddComponent<TextMeshProUGUI>();
            }
            moving = targetDatas.MoveVector;
            hittext.text = (targetDatas.HitScore != 0) ? targetDatas.HitScore.ToString() : "";
            if (targetDatas.HasVanishTime)
            {
                StartCoroutine(AutoVanish(targetDatas.VanishTime));
            }
            //ゲームオブジェクト初期化
            targetParentObject.SetActive(true);
            pointCanvasObject.gameObject.SetActive(false);
            targetModel.gameObject.SetActive(true);

            //イベントリスナーを登録
            receivers.Clear();
            foreach (var receiver in GetComponentsInChildren<IHitReceiver>(true))
            {
                receivers.Add(receiver);
            }
        }


        private void Start()
        {
            if (pointCanvasObject == null)
            {
                Debug.LogError("PointCanvas is not assigned in the inspector.");
            }
            if (targetDatas.TargetModel == null)
            {
                Debug.LogError("TargetModel is not assigned in the inspector.");
            }

        }

        private void OnEnable()
        {
            foreach (var receiver in GetComponentsInChildren<IHitReceiver>())
            {
                if(receiver == (IHitReceiver)this) continue;
                receivers.Add(receiver);
            }
            ManagerLocator.Instance.Phase.OnPhaseEnd += DisableObject;
        }

        private void OnDisable()
        {
            ManagerLocator.Instance.Phase.OnPhaseEnd -= DisableObject;
        }

        private void Update()
        {
            if (targetDatas == null || !targetDatas.IsMovable) return;
            if (isEnabled) return;

            if (time > targetDatas.MoveDurtation)
            {
                time = 0f;
                switch (targetDatas.MoveType)
                {
                    case MoveType.LinerMove:
                        break;
                    case MoveType.UFOMove:
                        int rx = Random.Range(0, 360);
                        int ry = Random.Range(0, 360);
                        int rz = Random.Range(0, 360);
                        moving = Quaternion.Euler(rx, ry, rz) * targetDatas.MoveVector;
                        break;
                    case MoveType.PendulumMove:
                        moving *= -1;
                        break;
                }
            }

            if (transform.localPosition.y < 0f && moving.y < 0f)
                moving = Vector3.Scale(moving, inverseY);

            transform.localPosition += moving * Time.deltaTime;
            time += Time.deltaTime;
        }

        public void OnHitNotify(IHitSender sender)
        {
            if (isEnabled) return;
            if (pointCanvasObject != null) pointCanvasObject.SetActive(true);
            //Debug.Log("hit by" + sender.ScoreCollector.ToString());
            //子供(VFXコントローラ)にもヒット通知を送る
            foreach (var receiver in receivers)
            {
                if (receiver == null) continue; // 破棄済みスキップ
                if (receiver == (IHitReceiver)this) continue;//自身スキップ
                receiver.OnHitNotify(sender);
            }
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            DisableObject();
            if (targetDatas != null)
                ManagerLocator.Instance.Game.AddScore(targetDatas.HitScore, targetDatas.ModelName);
        }

        private void DisableObject()
        {
            isEnabled = true;
            if (targetModel != null)
            {
                targetModel.gameObject.SetActive(false);
            }
            StartCoroutine(Release());
        }
        private IEnumerator AutoVanish(float vanishTime)
        {
            yield return new WaitForSeconds(vanishTime);
            if (this.gameObject != null)
                DisableObject();
        }
        private IEnumerator Release()
        {
            yield return waitThree;
            if (targetParentObject != null)
            {
                Destroy(targetParentObject);
            }
            ManagerLocator.Instance.CreateTarget.ReturnPool(this.gameObject);


        }


        private void OnTriggerEnter(Collider collision)
        {
            if (isEnabled) return; // 二重ヒット防止
                                   //MonobihabiorとGameObjetは別、要素が欲しければ、GetComponent<>();
            var hitSender = collision.gameObject.GetComponent<IHitSender>();
            if (hitSender != null)
                OnHitNotify(hitSender);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (isEnabled) return; // 二重ヒット防止
                                   //MonobihabiorとGameObjetは別、要素が欲しければ、GetComponent<>();
            var hitSender = collision.gameObject.GetComponent<IHitSender>();
            if (hitSender != null)
                OnHitNotify(hitSender);
        }

        [OnInspectorButton]
        private void EditorHit()
        {
            OnHitNotify(null);
        }

    }
}
