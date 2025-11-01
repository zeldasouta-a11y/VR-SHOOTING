using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class VRInputManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera targetCamera;
    [Range(1e-5f, 100f)] public float minFOV = 40f;
    [Range(1e-5f, 100f)] public float maxFOV = 100f;
    public float zoomSpeed = 1.5f;

    [Header("Left Hand Input")]
    public InputActionProperty leftTrigger;
    public InputActionProperty leftPrimary;
    public InputActionProperty leftSecondary;
    public InputActionProperty leftStick;

    [Header("Right Hand Input")]
    public InputActionProperty rightTrigger;
    public InputActionProperty rightPrimary;
    public InputActionProperty rightSecondary;
    public InputActionProperty rightStick;

    //登録しない場合はこう
    public SniperZoom zoom;
    [Header("UI Interaction")]
    public XRUIInputModule xrUIInputModule;

    private Vector2 moveValue;

    private void OnEnable()
    {
        EnableActions(true);
    }

    private void OnDisable()
    {
        EnableActions(false);
    }

    private void EnableActions(bool enable)
    {
        var list = new InputActionProperty[]
        {
            leftTrigger, leftPrimary, leftSecondary, leftStick,
            rightTrigger, rightPrimary, rightSecondary, rightStick
        };

        foreach (var action in list)
        {
            if (action.action == null) continue;
            if (enable)
            {
                action.action.performed += OnZoom;
                action.action.Enable(); 
            }
            else
            {
                action.action.performed -= OnZoom;
                action.action.Disable(); 
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
}
