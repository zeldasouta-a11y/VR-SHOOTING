using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCollisionManager : MonoBehaviour
{
    [SerializeField] GameObject pointCanvas;
    [HideInInspector] GameObject targetModel;
    [SerializeField]
    TargetData targetDatas;
    Vector3 moveing;
    private float time = 0f;

    /// <summary>
    /// �K���A�C���X�^���X�쐬����ɌĂԂ���
    /// </summary>
    /// <param name="score"></param>
    /// <param name="time"></param>
    public void Init(TargetData _data,GameObject Model = null)
    {
        targetDatas = _data;
        targetModel = Model;
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
        moveing = targetDatas.MoveVector;
        pointCanvas.SetActive(false);
        if (targetDatas.IsVanish) 
        {
            Destroy(this.gameObject, targetDatas.VanishTime);
        }
    }
    void Update()
    {
        if (!targetDatas.IsMovable) return;
        if (time > targetDatas.MoveDurtation)
        {
            time = 0f;
            if (targetDatas.IsUFOMove)
            {
                int rotationX = Random.Range(0, 360);
                int rotationY = Random.Range(0, 360);
                int rotationZ = Random.Range(0, 360);
                moveing = Quaternion.Euler(rotationX,rotationY,rotationZ) * targetDatas.MoveVector;
            }
            else if (targetDatas.IsPendulumMove)
            {
                moveing *= -1;
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
            targetModel.gameObject.SetActive(false);
            TextMeshProUGUI hittext = pointCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (hittext == null)
            {
                hittext = pointCanvas.AddComponent<TextMeshProUGUI>();
            }
            hittext.text = targetDatas.HitScore.ToString();
            pointCanvas.gameObject.SetActive(true);

            GameManager.Instance.AddScore(targetDatas.HitScore);
            
            Destroy(this.gameObject,3.0f);
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay Detected");
    }
}
