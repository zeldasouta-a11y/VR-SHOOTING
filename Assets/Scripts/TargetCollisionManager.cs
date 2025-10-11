using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCollisionManager : MonoBehaviour
{
    [SerializeField] private int hitcount = 0;
    [SerializeField] public GameObject targetModel;
    [SerializeField] GameObject pointCanvas;
    [SerializeField] private float vanishTime = 30.0f;
    
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
        Destroy(this.gameObject, vanishTime);
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
            hittext.text = hitcount.ToString();
            pointCanvas.SetActive(true);

            GameManager.Instance.AddScore(hitcount);
            
            Destroy(this.gameObject,3.0f);
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay Detected");
    }
}
