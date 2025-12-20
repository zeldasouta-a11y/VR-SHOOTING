using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using VRShooting.Item.Gun;
using VRShooting.Data;
using VRShooting.Item;
namespace VRShooting.Manager
{
    public class GunManager : MonoBehaviour
    {
        [SerializeField] private List<GunData> gundatas;
        private Dictionary<string,GunData> gunDataDic = new ();
        public IReadOnlyDictionary<string,GunData> GunDataDic => gunDataDic;
        private List<GunController> gunObjects = new ();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GunInitialize();
        }
        public void SetEvent(GameManager game,PhaseManager phase)
        {
            game.OnGameStart += GunRespawn;
        }
        private void OnDisable()
        {
            var game = ManagerLocator.Instance.Game;
            if (game != null)
            {
                game.OnGameStart -= GunRespawn;
            }
        }
        public void GunInitialize()
        {
            foreach (GunData data in gundatas)
            {
                Initialize(data);
            }
        }
        public void Initialize()
        {
            
        }
        private void Initialize(GunData data)
        {
            //GunController SetUp
            if (data.gunModelObject == null)
            {
                Debug.LogError("[GunManager] Invalid GunData or missing gunModelObject.");
                return;
            }

            // GunController�̏�����
            GunController gun = data.gunModelObject.GetComponent<GunController>();
            if (gun == null)
            {
                gun = data.gunModelObject.AddComponent<GunController>();
                Debug.LogWarning($"[GunManager] Added GunController to {data.gunModelObject.name}");
            }
            gunDataDic[data.Name] = data;
            //�����Q�Ɠn��
            gun.Init(data);
            AddController(gun);
        }
        public void AddController(GunController controller)
        {
            gunObjects.Add(controller);
        }
        private void GunRespawn()
        {
            foreach (IUsable gun in gunObjects)
            {
                gun.Respawn();
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

}
