using TMPro;
using UnityEngine;

public class TargetCollisionManager : MonoBehaviour
{
    private int hitcount = 0;
    [SerializeField] CameraManager cameraManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger Detected");
        string objecttag = collision.gameObject.tag;
        if (objecttag == "bullet")
        {
            hitcount = 100;
            GameObject hitclone = cameraManager.CreateInstance(this.transform.position+ new Vector3(0, 0.0f, 100.0f));
            TextMeshProUGUI hittext = hitclone.GetComponentInChildren<TextMeshProUGUI>();
            if (hittext == null)
            {
                hittext = hitclone.AddComponent<TextMeshProUGUI>();
            }
            hittext.text = hitcount.ToString();
            //Destroy(this.gameObject);
            Destroy(hitclone, 2.0f);
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay Detected");
    }
}
