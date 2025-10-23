using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateTargetManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject baseprefab;


    public event Action<GameObject, TargetData> OnTargetSpawned;
    
    private List<TargetData> targetModels = new List<TargetData>();
    RandomTable indexTable;
    RandomTable posTable;
    private void Awake()
    {
        mainCamera ??= Camera.main;
    }
    public void SetEvent(PhaseManager phaseManager,GameManager gameManager)
    {
        phaseManager.OnGamePhaseChanged += OnGamePhaseChangeHandle;
        phaseManager.OnCreateTime += OnCreateTimeHandle;
        gameManager.OnGameStart += OnGameStartHandle;
    }
    private void OnDisable()
    {
        var phaseManager = ManagerLocator.Instance.Phase;
        var gameManager = ManagerLocator.Instance.Game;
        if( phaseManager!= null)
        {
            phaseManager.OnGamePhaseChanged -= OnGamePhaseChangeHandle;
            phaseManager.OnCreateTime -= OnCreateTimeHandle;
        }
        if (gameManager != null)
        {
            gameManager.OnGameStart -= OnGameStartHandle;
        }
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
    private void OnGameStartHandle()
    {
        posTable = new RandomTable(ManagerLocator.Instance.Game.GameSeed);
        indexTable = new RandomTable(ManagerLocator.Instance.Game.GameSeed);
    }
    public GameObject CreateInstanceAndSetCameraAndScripts(int listIndex)
    {
        if (!IsValidIndex(listIndex)) return null;
        return CreateTarget(listIndex, GetRandomPosFromTable(targetModels[listIndex].MinPosition, targetModels[listIndex].MaxPosition));
    }
    

    [OnInspectorButton("Spawn Targets with Scripts")]
    private GameObject EditorSpawn(int listIndex, Vector3 localPosition)
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
        controller.Init(data, cloneModel,localPosition, mainCamera);
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

    
    private Vector3 GetRandomPosFromTable(Vector3 minPos, Vector3 maxPos)
    {
        return new Vector3
            (
            posTable.Range(minPos.x, maxPos.x),
            posTable.Range(minPos.y, maxPos.y),
            posTable.Range(minPos.z, maxPos.z)
            );
    }
    private void OnGamePhaseChangeHandle(PhaseSettingData phaseSetting)
    {
        if(phaseSetting.targetSettingSO.targetSettingData == null)
        {
            Debug.LogError("PhaseSettingData is Null!");
        }
        targetModels = phaseSetting.targetSettingSO.targetSettingData;

    }
    private void OnCreateTimeHandle()
    {
        CreateInstanceAndSetCameraAndScripts(indexTable.RangeInt(0, targetModels.Count));
    }
}

