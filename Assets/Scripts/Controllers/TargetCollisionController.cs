using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCollisionController : MonoBehaviour
{
    [SerializeField] GameObject pointCanvas;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI hittext;
    [HideInInspector] GameObject targetModel;
    [SerializeField] TargetData targetDatas;
    private Vector3 spawnPoint;
    private Vector3 moving;
    private readonly WaitForSeconds fixedUpdate = new WaitForSeconds(1f);
    private float time = 0f;
    private bool isEnabled = false;

    /// <summary>
    /// �K���A�C���X�^���X�쐬����ɌĂԂ���
    /// </summary>
    /// <param name="score"></param>
    /// <param name="time"></param>
    public void Init(TargetData _data, GameObject Model, Vector3 spawnAt, Camera targetCamera)
    {
        targetDatas = _data;
        targetModel = Model;
        spawnPoint = spawnAt;
        canvas.worldCamera = targetCamera;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pointCanvas == null)
        {
            Debug.LogError("PointCanvas is not assigned in the inspector.");
        }
        if (targetDatas.TargetModel == null)
        {
            Debug.LogError("TargetModel is not assigned in the inspector.");
        }
        moving = targetDatas.MoveVector;
        pointCanvas.SetActive(false);
        if (targetDatas.HasVanishTime)
        {
            Destroy(this.gameObject, targetDatas.VanishTime);
        }
    }
    private void OnEnable()
    {
        ManagerLocator.Instance.Phase.OnPhaseEnd += DisableObject;
        //StartCoroutine(ReturnToPoolAfterDelay());
    }
    private void OnDisable()
    {
        ManagerLocator.Instance.Phase.OnPhaseEnd -= DisableObject;
    }
    void Update()
    {
        if (!targetDatas.IsMovable) return;
        if (isEnabled) return;
        if (time > targetDatas.MoveDurtation)
        {
            time = 0f;
            switch (targetDatas.MoveType)
            {
                case MoveType.LinerMove:
                    break;
                case MoveType.UFOMove:
                    int rotationX = Random.Range(0, 360);
                    int rotationY = Random.Range(0, 360);
                    int rotationZ = Random.Range(0, 360);
                    moving = Quaternion.Euler(rotationX, rotationY, rotationZ) * targetDatas.MoveVector;
                    break;
                case MoveType.PendulumMove:
                    moving *= -1;
                    break;
            }

        }
        //UFO用,深くなったらひっくり返す
        if (this.gameObject.transform.localPosition.y < 0f && moving.y < 0f)
        {
            moving *= -1;
        }
        this.gameObject.transform.localPosition += moving * Time.deltaTime;
        time += Time.deltaTime;
    }
    public IEnumerator ReturnToPoolAfterDelay()
    {
        yield return null;
        if (!targetDatas.HasVanishTime) yield break;

        yield return new WaitForSeconds(targetDatas.VanishTime);
        this.gameObject.SetActive(false);
    }
    private void OnHit()
    {
        if (hittext == null)
        {
            hittext = pointCanvas.AddComponent<TextMeshProUGUI>();
        }
        hittext.text = (targetDatas.HitScore != 0)? targetDatas.HitScore.ToString(): "";
        pointCanvas.gameObject.SetActive(true);
        DisableObject();
    }
    private void DisableObject()
    {
        if (targetModel != null) targetModel.gameObject.SetActive(false);
        isEnabled = true;
        
        Destroy(this.gameObject, 3.0f);
    }
    void OnTriggerEnter(Collider collision)
    {
        string objecttag = collision.gameObject.tag;
        if (objecttag == "bullet")
        {
            OnHit();
            ManagerLocator.Instance.Game.AddScore(targetDatas.HitScore, targetDatas.ModelName);
        }
    }

}
