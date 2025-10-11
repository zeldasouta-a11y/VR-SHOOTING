using UnityEditor;
using UnityEngine;

public class CreateTargetSO : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject prefabObject;

    // Update is called once per frame
    public GameObject CreateInstanceAndSetCamera(Vector3 localPosition)
    {
        Debug.Log("CreateInstance");
        GameObject clone = Instantiate(prefabObject, localPosition, Quaternion.identity);
        Canvas canvas = clone.GetComponentInChildren<Canvas>();
        if (canvas == null) canvas = clone.AddComponent<Canvas>();
        canvas.worldCamera = mainCamera;
        return clone;
    }
}

[CustomEditor(typeof(CreateTargetSO))]
public class CameraManagerEditor : Editor
{

    private float x = 0;
    private float y = 0;
    private float z = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        Vector3 createAt = EditorGUILayout.Vector3Field("Position", new Vector3(x, y, z));
        x = createAt.x;
        y = createAt.y;
        z = createAt.z;
        CreateTargetSO cameraManager = (CreateTargetSO)target;
        if (GUILayout.Button($"Create Instance at {createAt}"))
        {
            cameraManager.CreateInstanceAndSetCamera(createAt);
        }
        serializedObject.ApplyModifiedProperties();
    }
}