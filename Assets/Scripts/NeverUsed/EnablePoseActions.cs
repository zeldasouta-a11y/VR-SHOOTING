using UnityEngine;
using UnityEngine.InputSystem;

public class EnablePoseActions : MonoBehaviour
{
    public InputActionReference position;
    public InputActionReference rotation;
    public InputActionReference tracking;

    void OnEnable()
    {
        position?.action?.Enable();
        rotation?.action?.Enable();
        tracking?.action?.Enable();
    }

    void OnDisable()
    {
        position?.action?.Disable();
        rotation?.action?.Disable();
        tracking?.action?.Disable();
    }
}
