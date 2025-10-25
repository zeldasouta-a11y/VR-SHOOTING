using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetController : MonoBehaviour
{
    [SerializeField] GameObject pointCanvasObject;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI hittext;
    [HideInInspector] GameObject targetModel;
    [SerializeField] TargetData targetDatas;
    private static WaitForSeconds waitThree = new WaitForSeconds(3.0f);
    private Vector3 inverseY = new Vector3(1, -1, 1);
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
        isEnabled = false;
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
        pointCanvasObject.gameObject.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        //UFO用,深くなったらYをひっくり返す
        if (this.gameObject.transform.localPosition.y < 0f && moving.y < 0f)
        {
            moving = Vector3.Scale(moving ,inverseY);
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
        pointCanvasObject.gameObject.SetActive(true);
        DisableObject();
        ManagerLocator.Instance.Game.AddScore(targetDatas.HitScore, targetDatas.ModelName);
    }
    private void DisableObject()
    {
        if (targetModel != null)
        {
            targetModel.gameObject.SetActive(false);
        } 
        isEnabled = true;
        StartCoroutine(Release());
    }
    private IEnumerator AutoVanish(float vanishTime) 
    {
        yield return new WaitForSeconds(vanishTime);
        if(this.gameObject != null)
        DisableObject();
    }
    private IEnumerator Release()
    {
        yield return waitThree;
        ManagerLocator.Instance.CreateTarget.ReturnPool(this.gameObject);
        if(targetModel != null) 
        {
            Destroy(targetModel);
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {
        string objecttag = collision.gameObject.tag;
        if (objecttag == "bullet")
        {
            OnHit();
        }
    }
    [OnInspectorButton]
    private void EditorHit()
    {
        OnHit();
    }
}
