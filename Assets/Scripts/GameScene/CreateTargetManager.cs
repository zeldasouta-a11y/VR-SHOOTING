using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateTargetManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject baseprefab;
    [SerializeField] private List<TargetData> targetModels;

    //[SerializeField] private List<TargetDeta> targetPrefabs;
    //public GameObject CreateInstanceAndSetCamera(int listIndex, Vector3 localPosition)
    //{
    //    Debug.Log("CreateInstance");
    //    GameObject clone = Instantiate(targetPrefabs[listIndex].TarGetModel, localPosition, Quaternion.identity);
    //    Canvas canvas = clone.GetComponentInChildren<Canvas>();
    //    if (canvas == null) canvas = clone.AddComponent<Canvas>();
    //    canvas.worldCamera = mainCamera;
    //    return clone;
    //}
    [OnInspectorButton("Spawn Targets with Scripts")]
    public GameObject CreateInstanceAndSetCameraAndScripts(int listIndex, Vector3 localPosition)
    {
        Debug.Log($"CreateInstance with Scripts,index:{listIndex} Positon:{localPosition}");
        GameObject cloneBase = Instantiate(baseprefab, localPosition, Quaternion.identity);
        GameObject cloneModel = Instantiate(targetModels[listIndex].TargetModel, localPosition, Quaternion.Euler(0,180,0), cloneBase.transform);
        //�R���C�_�[�ǉ�
        if (cloneModel.GetComponent<Collider>() == null) cloneModel.AddComponent<BoxCollider>();
        //�ŗL�l�ݒ�
        TargetCollisionManager _maneger = cloneBase.GetComponent<TargetCollisionManager>();
        _maneger.Init(targetModels[listIndex],cloneModel);
        //�J�����ݒ�
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

