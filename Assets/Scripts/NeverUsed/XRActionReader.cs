using UnityEngine;
using UnityEngine.InputSystem;

public class XRActionReader : MonoBehaviour
{
    public InputActionReference triggerAction; // Inspectorでアサイン

    private void OnEnable()
    {
        triggerAction.action.performed += OnTriggerPerformed;
        triggerAction.action.canceled += OnTriggerCanceled;
        triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPerformed;
        triggerAction.action.canceled -= OnTriggerCanceled;
        triggerAction.action.Disable();
    }

    private void OnTriggerPerformed(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        Debug.Log("Trigger pressed: " + value);
        // イベントを発火するなど
        OnTriggerValueChanged?.Invoke(value);
    }

    private void OnTriggerCanceled(InputAction.CallbackContext ctx)
    {
        OnTriggerValueChanged?.Invoke(0f);
    }

    public event System.Action<float> OnTriggerValueChanged;
}
