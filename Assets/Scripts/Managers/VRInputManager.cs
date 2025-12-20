using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace VRShooting.Manager
{
    public class VRInputManager : MonoBehaviour
    {
        [Header("Sniper Camera Settings")]
        public Camera targetCamera;
        [Range(1e-5f, 100f)] public float minFOV = 40f;
        [Range(1e-5f, 100f)] public float maxFOV = 100f;
        [SerializeField] float zoomSpeed = 1.5f;
        [Header("User Setting UI")]
        [SerializeField] GameObject userSettingPanel;
        [SerializeField] Button resetButton;
        [SerializeField] float panelDist = 1.5f;
        [SerializeField] Vector3 panelOffset = Vector3.zero;
        private bool isActive = false;
        [Header("Left Hand Input")]
        public InputActionProperty leftTrigger;
        public InputActionProperty leftPrimary;
        public InputActionProperty leftSecondary;
        public InputActionProperty leftStick;
        public InputActionProperty leftXButton;
        public InputActionProperty leftYButton;

        [Header("Right Hand Input")]
        public InputActionProperty rightTrigger;
        public InputActionProperty rightPrimary;
        public InputActionProperty rightSecondary;
        public InputActionProperty rightStick;
        public InputActionProperty rightAButton;
        public InputActionProperty rightBButton;

        [Header("Key Board Input")]
        [SerializeField] bool isUseKeyBoard = false;
        public InputActionProperty settingButton;
        //登録しない場合はこうやって手動登録
        public SniperZoom zoom;
        [Header("UI Interaction")]
        public XRUIInputModule xrUIInputModule;

        [Header("Player Camera")]
        [SerializeField] private Transform player;
        private Vector2 moveValue;
        private void Start()
        {
            if (userSettingPanel == null)
            {
                Debug.LogError("userPanel is not Exist");
            }
            userSettingPanel.SetActive(false);
        }
        public void SetEvent(GameManager game)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(() => game.GameRestart());
        }
        private void OnEnable()
        {
            settingButton.action.performed += OnSettingOpen;
            leftTrigger.action.performed += OnZoom;
            rightTrigger.action.performed += OnZoom;
            leftXButton.action.performed += OnSettingOpen;
            rightAButton.action.performed += OnSettingOpen;
            EnableActions(true);
        }

        private void OnDisable()
        {
            settingButton.action.performed -= OnSettingOpen;
            leftTrigger.action.performed -= OnZoom;
            rightTrigger.action.performed -= OnZoom;
            leftXButton.action.performed -= OnSettingOpen;
            rightAButton.action.performed -= OnSettingOpen;
            EnableActions(false);
        }

        private void EnableActions(bool enable)
        {
            var list = new InputActionProperty[]
            {
            leftTrigger, leftPrimary, leftSecondary, leftStick,leftXButton,leftYButton,
            rightTrigger, rightPrimary, rightSecondary, rightStick,rightAButton,rightBButton
            };

            foreach (var action in list)
            {
                if (action.action == null) continue;
                if (enable)
                {
                    action.action.performed += OnTrigger;
                    action.action.canceled += OnTriggerCanceled;
                    action.action.Enable();
                }
                else
                {
                    action.action.performed -= OnTrigger;
                    action.action.canceled -= OnTriggerCanceled;
                    action.action.Disable();
                }
            }

            if (isUseKeyBoard)
            {
                if (enable)
                {
                    settingButton.action.performed += OnTrigger;
                    settingButton.action.canceled += OnTriggerCanceled;
                    settingButton.action.Enable();
                }
                else
                {
                    settingButton.action.performed -= OnTrigger;
                    settingButton.action.canceled -= OnTriggerCanceled;
                    settingButton.action.Disable();
                }

            }
        }

        private void Update()
        {
            // スティック入力でFOVをズーム（右手のみ）
            moveValue = rightTrigger.action?.ReadValue<Vector2>() ?? Vector2.zero;
            targetCamera.fieldOfView -= moveValue.y * zoomSpeed;
            targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView, minFOV, maxFOV);
        }

        private void HandleInput(InputActionProperty actionProp, string name)
        {
            if (actionProp.action == null) return;
            Vector2 val = actionProp.action.ReadValue<Vector2>();

        }
        private void OnZoom(InputAction.CallbackContext context)
        {
            moveValue = context.ReadValue<Vector2>();

        }
        //lefttrigger.action.canceld += で呼べば、毎フレーム観測しなくていい
        private void OnSettingOpen(InputAction.CallbackContext context)
        {
            isActive = !isActive;
            if (isActive)
            {
                //プレイヤーの前方 
                Vector3 foward = player.forward;
                //プレイヤーの前方 + プレイヤーの向きベクトル*距離 + 高さ
                Vector3 targetPos = player.position + foward.normalized * panelDist + panelOffset;
                //パネルの場所-プレイヤーの場所で向きを作る(関数で向きに変換)
                Vector3 lookDir = targetPos - player.position;
                lookDir.y = 0;

                userSettingPanel.transform.SetPositionAndRotation(targetPos, Quaternion.LookRotation(lookDir));
            }
            userSettingPanel.SetActive(isActive);
        }
        private void OnTrigger(InputAction.CallbackContext context)
        {
            //Debug.Log(context.ToString());
        }
        private void OnTriggerCanceled(InputAction.CallbackContext context)
        {
            //Debug.Log(context.ToString());
        }
    }

}
