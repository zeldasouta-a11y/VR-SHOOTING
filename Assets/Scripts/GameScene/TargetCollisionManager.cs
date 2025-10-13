using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCollisionManager : MonoBehaviour
{
    [SerializeField] GameObject pointCanvas;
    [SerializeField]
    TargetData targetDatas;
    Vector3 moveing;
    private float time = 0f;

    /// <summary>
    /// 必ず、インスタンス作成直後に呼ぶこと
    /// </summary>
    /// <param name="score"></param>
    /// <param name="time"></param>
    public void Init(TargetData _data)
    {
        targetDatas = _data;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pointCanvas == null)
        {
            Debug.LogError("PointCanvas is not assigned in the inspector.");
        }
        if (targetDatas.targetModel == null)
        {
            Debug.LogError("TargetModel is not assigned in the inspector.");
        }
        moveing = targetDatas.moveVector;
        pointCanvas.SetActive(false);
        if (targetDatas.isVanish) 
        {
            Destroy(this.gameObject, targetDatas.vanishTime);
        }
    }
    void Update()
    {
        if (!targetDatas.isMovable) return;
        if (time > targetDatas.moveDurtation)
        {
            time = 0f;
            if (targetDatas.isUFOMove)
            {
                int rotationX = Random.Range(0, 360);
                int rotationY = Random.Range(0, 360);
                int rotationZ = Random.Range(0, 360);
                moveing = Quaternion.Euler(rotationX,rotationY,rotationZ) * targetDatas.moveVector;
            }
            else if (targetDatas.isPendulumMove)
            {
                moveing = -targetDatas.moveVector;
            }
        }
        this.gameObject.transform.localPosition += moveing * Time.deltaTime;
        time += Time.deltaTime;
    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger Detected");
        string objecttag = collision.gameObject.tag;
        if (objecttag == "bullet")
        {
            targetDatas.targetModel.SetActive(false);
            TextMeshProUGUI hittext = pointCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (hittext == null)
            {
                hittext = pointCanvas.AddComponent<TextMeshProUGUI>();
            }
            hittext.text = targetDatas.hitScore.ToString();
            pointCanvas.SetActive(true);

            GameManager.Instance.AddScore(targetDatas.hitScore);
            
            Destroy(this.gameObject,3.0f);
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay Detected");
    }
}
