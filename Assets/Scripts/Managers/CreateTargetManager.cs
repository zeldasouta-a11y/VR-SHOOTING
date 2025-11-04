using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public enum SpawnChooseType { Random,MaxSpawn,SpawnWeight}
public class CreateTargetManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject baseprefab;
    [SerializeField] int initializePoolSize = 10;
    [SerializeField] bool usePool = false;
    [SerializeField] TargetDataSO defaultTargets;

    public event Action<GameObject, TargetData> OnTargetSpawned;
    private Queue<GameObject> targetPool = new Queue<GameObject>();
    private Queue<int> spawnIndexQueue = new();
    private List<TargetData> targetModels = new();
    private SpawnChooseType chooseType;
    RandomTable indexTable;
    RandomTable posTable;
    private void Awake()
    {
        mainCamera ??= Camera.main;
    }
    private void Start()
    {
        for(int i = 0; i < initializePoolSize; i++)
        {
            GameObject clone = Instantiate(baseprefab);
            clone.SetActive(false);
            targetPool.Enqueue(clone);
        }   
    }
    public void SetEvent(PhaseManager phaseManager,GameManager gameManager)
    {
        phaseManager.OnPhaseChanged += OnGamePhaseChangeHandle;
        phaseManager.OnCreateTime += OnCreateTimeHandle;
        gameManager.OnGameStart += OnGameStartHandle;
    }
    private void OnDisable()
    {
        var phaseManager = ManagerLocator.Instance.Phase;
        var gameManager = ManagerLocator.Instance.Game;
        if( phaseManager!= null)
        {
            phaseManager.OnPhaseChanged -= OnGamePhaseChangeHandle;
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
    public GameObject CreateTergetRandomPos(int listIndex)
    {
        if (!IsValidIndex(listIndex)) return null;
        if(usePool) return CreateTargetFromPool(listIndex, GetRandomPosFromTable(targetModels[listIndex].MinPosition, targetModels[listIndex].MaxPosition));
        else return CreateTarget(listIndex, GetRandomPosFromTable(targetModels[listIndex].MinPosition, targetModels[listIndex].MaxPosition));
    }
    

    [OnInspectorButton("Spawn Targets with Scripts")]
    private GameObject EditorSpawn(int listIndex, Vector3 localPosition)
    {
        var data = defaultTargets.targetSettingData[listIndex];
        GameObject cloneBase = (targetPool.Count > 0) ? targetPool.Dequeue() : Instantiate(baseprefab);
        cloneBase.transform.SetPositionAndRotation(localPosition, Quaternion.Euler(0,0,0));

        GameObject cloneModel = Instantiate(data.TargetModel, cloneBase.transform);
        cloneModel.transform.localPosition = Vector3.zero;
        cloneModel.transform.localRotation = Quaternion.Euler(0, 180, 0);

        var controller = cloneBase.GetComponent<TargetController>();
        cloneBase.SetActive(true);//先に起動しないとコルーチンが発動しない
        controller.Init(data, cloneModel, localPosition, mainCamera);
        
        return cloneBase;
    }

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

        var controller = cloneBase.GetComponent<TargetController>();
        controller.Init(data, cloneModel,localPosition, mainCamera);
        return cloneBase;
    }
    private GameObject CreateTargetFromPool(int listIndex, Vector3 localPosition)
    {
        if (!IsValidIndex(listIndex)) return null;
        var data = targetModels[listIndex];
        GameObject cloneBase = (targetPool.Count > 0) ? targetPool.Dequeue() : Instantiate(baseprefab);
        cloneBase.transform.SetPositionAndRotation(localPosition, Quaternion.Euler(0,0,0));

        GameObject cloneModel = Instantiate(data.TargetModel, cloneBase.transform);
        cloneModel.transform.localPosition = Vector3.zero;
        cloneModel.transform.localRotation = Quaternion.Euler(0, 180, 0);

        var controller = cloneBase.GetComponent<TargetController>();
        cloneBase.SetActive(true);//先に起動しないとコルーチンが発動しない
        controller.Init(data, cloneModel, localPosition, mainCamera);
        
        return cloneBase;
    }
    public void ReturnPool(GameObject obj)
    {
        Rigidbody rbody = obj.GetComponent<Rigidbody>();
        //物理演算リセット
        if (rbody != null) 
        {
            rbody.linearVelocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;
        }
        obj.SetActive(false);
        targetPool.Enqueue(obj);
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
        if (phaseSetting.targetSettingSO.targetSettingData == null)
        {
            Debug.LogError("PhaseSettingData is Null!");
        }
        targetModels = phaseSetting.targetSettingSO.targetSettingData;
        chooseType = phaseSetting.spawnChoose;
        spawnIndexQueue = ManagerLocator.Instance.Phase.CustomIndexQueue;

    }
    private void OnCreateTimeHandle()
    {
        int index;
        switch (chooseType)
        {
            case SpawnChooseType.Random:
                CreateTergetRandomPos(indexTable.RangeInt(0, targetModels.Count));
                break;
            case SpawnChooseType.MaxSpawn:
                if (spawnIndexQueue.Count == 0) return;
                index = spawnIndexQueue.Dequeue();
                CreateTergetRandomPos(index);
                break;
            case SpawnChooseType.SpawnWeight:
                if (spawnIndexQueue.Count == 0) return;
                index = spawnIndexQueue.Dequeue();
                CreateTergetRandomPos(index);
                spawnIndexQueue.Enqueue(index);
                break;
        }


    }
}

