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
    private VFXController controller;
    private readonly WaitForSeconds fixedUpdate = new WaitForSeconds(1f);
    private float time = 0f;
    private bool isEnabled = false;

   

    public void Init(TargetData data, GameObject model, Vector3 spawnAt, Camera targetCamera)
    {
        targetDatas = data;
        targetModel = model;
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
        controller = GetComponentInChildren<VFXController>();
        
    }

    private void OnEnable()
    {
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

    public IEnumerator ReturnToPoolAfterDelay()
    {
        yield return null;
        if (targetDatas == null || !targetDatas.HasVanishTime) yield break;

        yield return new WaitForSeconds(targetDatas.VanishTime);
        gameObject.SetActive(false);
    }

    private void OnHit()
    {
        if (hittext == null && pointCanvasObject != null)
            hittext = pointCanvasObject.AddComponent<TextMeshProUGUI>();

        if (hittext != null)
            hittext.text = (targetDatas != null && targetDatas.HitScore != 0) ? targetDatas.HitScore.ToString() : "";

        if (pointCanvasObject != null) pointCanvasObject.SetActive(true);
        if(controller != null)
        {
            controller.SpawnBreakFx();
            controller.PlayBreakSfx();
        }
        

        DisableObject();
        if (targetDatas != null)
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
        
        if (targetModel != null) targetModel.SetActive(false);
        isEnabled = true;
        Destroy(gameObject, 3.0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isEnabled) return;                     // 二重ヒット防止
        if (collision.gameObject.tag == "bullet")
            OnHit();
    }

    [OnInspectorButton]
    private void EditorHit()
    {
        OnHit();
    }

    
}