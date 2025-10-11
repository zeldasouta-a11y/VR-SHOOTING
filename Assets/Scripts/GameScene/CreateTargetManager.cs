using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateTargetManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject baseprefab;
    [SerializeField] private List<GameObject> prefabModels;
    [SerializeField] private List<GameObject> targetPrefabs;

    // Start is called before the first frame update

    public GameObject CreateInstanceAndSetCamera(int listIndex, Vector3 localPosition)
    {
        Debug.Log("CreateInstance");
        GameObject clone = Instantiate(targetPrefabs[listIndex], localPosition, Quaternion.identity);
        Canvas canvas = clone.GetComponentInChildren<Canvas>();
        if (canvas == null) canvas = clone.AddComponent<Canvas>();
        canvas.worldCamera = mainCamera;
        return clone;
    }
    public GameObject CreateInstanceAndSetCameraAndScripts(int listIndex, Vector3 localPosition)
    {
        Debug.Log("CreateInstance with Scripts");
        GameObject cloneBase = Instantiate(baseprefab, localPosition, Quaternion.identity);
        GameObject cloneModel = Instantiate(prefabModels[listIndex], localPosition, Quaternion.Euler(0,180,0), cloneBase.transform);
        if (cloneModel.GetComponent<Collider>() == null) cloneModel.AddComponent<BoxCollider>();

        TargetCollisionManager _maneger = cloneBase.GetComponent<TargetCollisionManager>();
        _maneger.targetModel = cloneModel;
        Canvas canvas = cloneBase.GetComponentInChildren<Canvas>();
        if (canvas == null) canvas = cloneBase.AddComponent<Canvas>();
        canvas.worldCamera = mainCamera;
        return cloneBase;
    }
}

[CustomEditor(typeof(CreateTargetManager))]
public class CreateTargetManagerEditor : Editor
{
    private int listIndex = 0;
    private float x = 0;
    private float y = 0;
    private float z = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        listIndex = EditorGUILayout.IntField("Prefab Index", listIndex);
        Vector3 createAt = EditorGUILayout.Vector3Field("Position", new Vector3(x, y, z));
        x = createAt.x;
        y = createAt.y;
        z = createAt.z;
        serializedObject.Update();
        CreateTargetManager manager = (CreateTargetManager)target;
        if (GUILayout.Button("Spawn Targets"))
        {
            manager.CreateInstanceAndSetCamera(listIndex, createAt);
        }
        if (GUILayout.Button("Spawn Targets with Scripts"))
        {
            manager.CreateInstanceAndSetCameraAndScripts(listIndex, createAt);
        }
        serializedObject.ApplyModifiedProperties();
    }
}