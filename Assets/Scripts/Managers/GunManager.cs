using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using VRShooting.Weapon;
using VRShooting.Data;
namespace VRShooting.Manager
{
    public class GunManager : MonoBehaviour
    {
        [SerializeField] private List<GunData> gundatas;
        private List<GunController> gunObjects = new ();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GunInitialize();
        }
        public void SetEvent(GameManager game,PhaseManager phase)
        {
            game.OnGameStart += GunRespawn;
            phase.OnPhaseChanged += GunSpawn;
        }
        private void OnDisable()
        {
            var game = ManagerLocator.Instance.Game;
            if (game != null)
            {
                game.OnGameStart -= GunRespawn;
            }
            var phase = ManagerLocator.Instance.Phase;
            if(phase != null){
                phase.OnPhaseChanged -= GunSpawn;
            }
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
                    Debug.LogWarning($"[GunManager] Added GunController to {data.gunModelObject.name}");
                }
                //�����Q�Ɠn��
                gun.Init(data);
                gunObjects.Add(gun);
            }
        }
        private void GunRespawn()
        {
            foreach (GunController gun in gunObjects)
            {
                gun.GunRespawn();
            }
        }
        private void GunSpawn(PhaseSettingData data)
        {
            if(data.gamePhase == PhaseState.AttashCase)
            {
                GunRespawn();
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
