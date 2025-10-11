using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCollisionManager : MonoBehaviour
{
    [SerializeField] public GameObject targetModel;
    [SerializeField] GameObject pointCanvas;
    [SerializeField] private int hitScore = 0;
    [SerializeField] private bool isVanish = false;
    [SerializeField] private float vanishTime = 30.0f;


    /// <summary>
    /// 必ず、インスタンス作成直後に呼ぶこと
    /// </summary>
    /// <param name="score"></param>
    /// <param name="time"></param>
    public void Init(GameObject model,int score,bool isvanish,float time)
    {
        targetModel = model;
        hitScore = score;
        isVanish = isvanish;
        vanishTime = time;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pointCanvas == null)
        {
            Debug.LogError("PointCanvas is not assigned in the inspector.");
        }
        if (targetModel == null)
        {
            Debug.LogError("TargetModel is not assigned in the inspector.");
        }
        pointCanvas.SetActive(false);
        if (isVanish) 
        {
            Destroy(this.gameObject, vanishTime);
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger Detected");
        string objecttag = collision.gameObject.tag;
        if (objecttag == "bullet")
        {
            targetModel.SetActive(false);
            TextMeshProUGUI hittext = pointCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (hittext == null)
            {
                hittext = pointCanvas.AddComponent<TextMeshProUGUI>();
            }
            hittext.text = hitScore.ToString();
            pointCanvas.SetActive(true);

            GameManager.Instance.AddScore(hitScore);
            
            Destroy(this.gameObject,3.0f);
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay Detected");
    }
}
