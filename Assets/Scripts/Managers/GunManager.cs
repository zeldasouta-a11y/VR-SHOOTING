using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GunManager : MonoBehaviour
{
    [SerializeField] private List<GunData> gundatas;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       GunInitialize();
    }
    public void GunInitialize()
    {
        Initialize();
    }
    private void Initialize()
    {
        foreach (GunData data in gundatas)
        {
            //GunController SetUp
            if (data == null || data.gunModelObject == null)
            {
                Debug.LogError("[GunManager] Invalid GunData or missing gunModelObject.");
                continue;
            }

            // GunController�̏�����
            GunController gun = data.gunModelObject.GetComponent<GunController>();
            if (gun == null)
            {
                gun = data.gunModelObject.AddComponent<GunController>();
                Debug.Log($"[GunManager] Added GunController to {data.gunModelObject.name}");
            }
            //�����Q�Ɠn��
            gun.Init(data);

        }
    }
    [OnInspectorButton]
    public void GunActive(int index)
    {
        GunController controller = gundatas[index].gunModelObject.GetComponent<GunController>();
        if (controller == null) Debug.LogError("Gun is not assinged");
        ActivateEventArgs args = new ActivateEventArgs();
        controller.Activate(args);
    }
//#if UNITY_EDITOR
//    [OnInspectorButton("ReStart",true)]
//    private void EditorRestart()
//    {
//        Initialize();
//    }
//#endif
}
