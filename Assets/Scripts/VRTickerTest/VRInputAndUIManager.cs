using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRInputAndUIManager : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionProperty triggerAction;        // 例: "RightHand/Trigger"
    public InputActionProperty primaryButtonAction;  // 例: "RightHand/PrimaryButton" （A/B など）
    public InputActionProperty secondaryButtonAction; // 例: "RightHand/SecondaryButton"
    public InputActionProperty joystickAction;       // 例: "RightHand/Joystick" の Vector2

    [Header("UI")]
    public XRUIInputModule xruiInputModule;          // シーン上の XR UI Input Module
    public XRRayInteractor uiRayInteractor;           // UI に使っている Ray Interactor

    private void OnEnable()
    {
        triggerAction.action.Enable();
        primaryButtonAction.action.Enable();
        secondaryButtonAction.action.Enable();
        joystickAction.action.Enable();
    }

    private void OnDisable()
    {
        triggerAction.action.Disable();
        primaryButtonAction.action.Disable();
        secondaryButtonAction.action.Disable();
        joystickAction.action.Disable();
    }

    private void Update()
    {
        // --- ゲーム入力として読み取る例 ---
        float triggerValue = triggerAction.action.ReadValue<float>();
        Vector2 stickValue = joystickAction.action.ReadValue<Vector2>();
        bool primaryPressed = primaryButtonAction.action.WasPressedThisFrame();
        bool secondaryPressed = secondaryButtonAction.action.WasPressedThisFrame();

        // 例: ログ表示
        Debug.Log($"Trigger: {triggerValue:F2}, Stick: {stickValue}, A: {primaryPressed}, B: {secondaryPressed}");

        // 例: UI 入力をチェック（UI にフォーカスが当たっているかどうか、現在のレイキャスト先オブジェクトなど）
        // ここでは、XRUIInputModule の現在のレイキャスト先を取得する方法の一例：
        uiRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycast);
        if (raycast.isValid && raycast.gameObject != null)
        {
            GameObject hit = raycast.gameObject;
            // UI 要素（たとえば Button）の操作など
            // 例えば、トリガーが押されたら UI 要素をクリックさせる処理を入れる、など
        }
    }
}
