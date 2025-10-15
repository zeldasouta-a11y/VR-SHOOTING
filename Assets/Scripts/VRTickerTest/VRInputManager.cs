using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class VRInputManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera targetCamera;
    [Range(30f, 120f)] public float minFOV = 40f;
    [Range(30f, 120f)] public float maxFOV = 100f;
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

    [Header("UI Interaction")]
    public XRUIInputModule xrUIInputModule;

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
            if (enable) action.action.Enable();
            else action.action.Disable();
        }
    }

    private void Update()
    {
        HandleInput(leftTrigger, "LeftTrigger");
        HandleInput(rightTrigger, "RightTrigger");

        // スティック入力でFOVをズーム（右手のみ）
        Vector2 rightJoy = rightStick.action?.ReadValue<Vector2>() ?? Vector2.zero;
        if (Mathf.Abs(rightJoy.y) > 0.1f)
        {
            targetCamera.fieldOfView -= rightJoy.y * zoomSpeed;
            targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView, minFOV, maxFOV);
        }
    }

    private void HandleInput(InputActionProperty actionProp, string name)
    {
        if (actionProp.action == null) return;
        float val = actionProp.action.ReadValue<float>();
        if (val > 0.9f)
        {
            Debug.Log($"{name} pressed");
        }
    }
}
