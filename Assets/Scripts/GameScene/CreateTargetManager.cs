using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateTargetManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject baseprefab;
    [SerializeField] private List<TargetData> targetModels;
    //[SerializeField] private List<TargetDeta> targetPrefabs;

    // Start is called before the first frame update

    //public GameObject CreateInstanceAndSetCamera(int listIndex, Vector3 localPosition)
    //{
    //    Debug.Log("CreateInstance");
    //    GameObject clone = Instantiate(targetPrefabs[listIndex].TarGetModel, localPosition, Quaternion.identity);
    //    Canvas canvas = clone.GetComponentInChildren<Canvas>();
    //    if (canvas == null) canvas = clone.AddComponent<Canvas>();
    //    canvas.worldCamera = mainCamera;
    //    return clone;
    //}
    public GameObject CreateInstanceAndSetCameraAndScripts(int listIndex, Vector3 localPosition)
    {
        Debug.Log("CreateInstance with Scripts");
        GameObject cloneBase = Instantiate(baseprefab, localPosition, Quaternion.identity);
        GameObject cloneModel = Instantiate(targetModels[listIndex].targetModel, localPosition, Quaternion.Euler(0,180,0), cloneBase.transform);
        //コライダー追加
        if (cloneModel.GetComponent<Collider>() == null) cloneModel.AddComponent<BoxCollider>();
        //固有値設定
        TargetCollisionManager _maneger = cloneBase.GetComponent<TargetCollisionManager>();
        _maneger.Init(cloneModel, targetModels[listIndex].hitScore, targetModels[listIndex].isVanish, targetModels[listIndex].vanishTime);
        //カメラ設定
        Canvas canvas = cloneBase.GetComponentInChildren<Canvas>();
        if (canvas == null) canvas = cloneBase.AddComponent<Canvas>();
        canvas.worldCamera = mainCamera;

        return cloneBase;
    }

    private void Reset()
    {
        if (mainCamera == null)
        {
            GameObject cameraObject = GameObject.FindWithTag("MainCamera");
            if (cameraObject != null) 
            {
                mainCamera = cameraObject.GetComponent<Camera>();
            }
        }
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
        //if (GUILayout.Button("Spawn Targets"))
        //{
        //    manager.CreateInstanceAndSetCamera(listIndex, createAt);
        //}
        if (GUILayout.Button("Spawn Targets with Scripts"))
        {
            manager.CreateInstanceAndSetCameraAndScripts(listIndex, createAt);
        }
        serializedObject.ApplyModifiedProperties();
    }
}