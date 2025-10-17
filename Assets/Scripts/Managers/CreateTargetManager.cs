using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateTargetManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject baseprefab;
    [Header("Mode Data ScriptableObject")]
    [SerializeField] private List<TargetDataSO> targetsList;
    [SerializeField] private TargetDataSO EasyModeTargetData;
    [SerializeField] private TargetDataSO NormalModeTargetData;
    [SerializeField] private TargetDataSO HardModeTargetData;

    public event Action<GameObject, TargetData> OnTargetSpawned;

    private List<TargetData> targetModels ;
    private int listIndex = 0;
    RandomTable indexTable;
    RandomTable posTable;
    private void Awake()
    {
        mainCamera ??= Camera.main;
    }
    private void Start()
    {
        targetModels = targetsList[listIndex].targetSettingData;
        posTable = new RandomTable(ManagerLocator.Instance.Game.GameSeed);
        indexTable = new RandomTable(ManagerLocator.Instance.Game.GameSeed);
        ManagerLocator.Instance.Game.OnGameModeChanged += OnGamePhaseChangeHandle;
        ManagerLocator.Instance.Game.OnCreateTime += OnCreateTimeHandle;
    }
    private void OnDisable()
    {
        ManagerLocator.Instance.Game.OnGameModeChanged -= OnGamePhaseChangeHandle;
        ManagerLocator.Instance.Game.OnCreateTime -= OnCreateTimeHandle;
    }

    public GameObject CreateInstanceAndSetCameraAndScripts(int listIndex)
    => CreateTarget(listIndex, GetRandomPosFromTable(targetModels[listIndex].MinPosition, targetModels[listIndex].MaxPosition));

    [OnInspectorButton("Spawn Targets with Scripts")]
    public GameObject CreateInstanceAndSetCameraAndScripts(int listIndex, Vector3 localPosition)
        => CreateTarget(listIndex, localPosition);

    private GameObject CreateTarget(int listIndex, Vector3 localPosition)
    {
        if (!IsValidIndex(listIndex)) return null;
        var data = targetModels[listIndex];

        GameObject cloneBase = Instantiate(baseprefab, localPosition, Quaternion.identity);
        GameObject cloneModel = Instantiate(data.TargetModel, cloneBase.transform);
        cloneModel.transform.localPosition = Vector3.zero;
        cloneModel.transform.localRotation = Quaternion.Euler(0, 180, 0);

        if (cloneModel.GetComponent<Collider>() == null)
            cloneModel.AddComponent<BoxCollider>();

        var controller = cloneBase.GetComponent<TargetCollisionController>();
        controller.Init(data, cloneModel, mainCamera);
        return cloneBase;
    }
    private bool IsValidIndex(int index)
    {
        if (targetModels == null || targetModels.Count == 0)
        {
            Debug.LogError("TargetModelsが設定されていません。");
            return false;
        }
        if (index < 0 || index >= targetModels.Count)
        {
            Debug.LogError($"Index {index} は範囲外です（0〜{targetModels.Count - 1}）");
            return false;
        }
        if (targetModels[index].TargetModel == null)
        {
            Debug.LogError($"TargetModel[{index}] が未設定です。");
            return false;
        }
        return true;
    }

    private void Reset()
    {
        if (mainCamera == null)
        {
            //FindAndSetMainCamera
            GameObject cameraObject = GameObject.FindWithTag("MainCamera");
            if (cameraObject != null) 
            {
                mainCamera = cameraObject.GetComponent<Camera>();
            }
        }
    }
    private Vector3 GetRandomPosFromTable(Vector3 minPos, Vector3 maxPos)
    {
        return new Vector3
            (
            posTable.Range(minPos.x, maxPos.x),
            posTable.Range(minPos.y, maxPos.y),
            posTable.Range(minPos.z, maxPos.z)
            );
    }
    private void OnGamePhaseChangeHandle(GameMode mode)
    {
        if(listIndex < targetsList.Count)
        {
            targetModels = targetsList[++listIndex].targetSettingData;
        }
    }
    private void OnCreateTimeHandle()
    {
        CreateInstanceAndSetCameraAndScripts(indexTable.RangeInt(0, targetModels.Count));
    }
}

