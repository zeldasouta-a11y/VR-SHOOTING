using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject prefabObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public GameObject CreateInstance(Vector3 localPosition)
    {
        Debug.Log("CreateInstance");
        GameObject clone = Instantiate(prefabObject, localPosition, Quaternion.identity);
        Canvas canvas = clone.GetComponentInChildren<Canvas>();
        if (canvas == null) return null;
        canvas.worldCamera = mainCamera;
        return clone;
    }
}
