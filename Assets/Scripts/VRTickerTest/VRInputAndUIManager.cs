using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRInputAndUIManager : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionProperty triggerAction;        // ��: "RightHand/Trigger"
    public InputActionProperty primaryButtonAction;  // ��: "RightHand/PrimaryButton" �iA/B �Ȃǁj
    public InputActionProperty secondaryButtonAction; // ��: "RightHand/SecondaryButton"
    public InputActionProperty joystickAction;       // ��: "RightHand/Joystick" �� Vector2

    [Header("UI")]
    public XRUIInputModule xruiInputModule;          // �V�[����� XR UI Input Module
    public XRRayInteractor uiRayInteractor;           // UI �Ɏg���Ă��� Ray Interactor

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
        // --- �Q�[�����͂Ƃ��ēǂݎ��� ---
        float triggerValue = triggerAction.action.ReadValue<float>();
        Vector2 stickValue = joystickAction.action.ReadValue<Vector2>();
        bool primaryPressed = primaryButtonAction.action.WasPressedThisFrame();
        bool secondaryPressed = secondaryButtonAction.action.WasPressedThisFrame();

        // ��: ���O�\��
        Debug.Log($"Trigger: {triggerValue:F2}, Stick: {stickValue}, A: {primaryPressed}, B: {secondaryPressed}");

        // ��: UI ���͂��`�F�b�N�iUI �Ƀt�H�[�J�X���������Ă��邩�ǂ����A���݂̃��C�L���X�g��I�u�W�F�N�g�Ȃǁj
        // �����ł́AXRUIInputModule �̌��݂̃��C�L���X�g����擾������@�̈��F
        uiRayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycast);
        if (raycast.isValid && raycast.gameObject != null)
        {
            GameObject hit = raycast.gameObject;
            // UI �v�f�i���Ƃ��� Button�j�̑���Ȃ�
            // �Ⴆ�΁A�g���K�[�������ꂽ�� UI �v�f���N���b�N�����鏈��������A�Ȃ�
        }
    }
}
