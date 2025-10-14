using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunManager : MonoBehaviour
{
    [SerializeField] private List<GunData> gundatas;
    [SerializeField] private bool isFullAuto;

    public List<GunData> GetGunDataList() => gundatas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GunData data in gundatas)
        {
            GunController controller = data.gunModelObject.GetComponent<GunController>();
            if (controller == null) controller = data.gunModelObject.AddComponent<GunController>();
            controller.Init(data);
        }
    }

    [OnInspectorButton]
    public void GunActive(int index)
    {
        GunController controller = gundatas[index].gunModelObject.GetComponent<GunController>();
        if (controller == null) Debug.Log("Gun is not assinged");
        ActivateEventArgs args = new ActivateEventArgs();
        controller.Activate(args);
    }
}
