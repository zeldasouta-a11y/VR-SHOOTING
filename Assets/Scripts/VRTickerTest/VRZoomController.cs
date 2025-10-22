using UnityEngine;

using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

// Example of using an InputActionMap named "Player" from a UnityEngine.MonoBehaviour implementing callback interface.
 public class VRZoomController : MonoBehaviour
 {
    public InputActionProperty moveAction; // 右手スティック入力
    public float moveSpeed = 1.5f;

    private XROrigin xrOrigin;
    private CharacterController character;

    void Start()
    {
        xrOrigin = GetComponent<XROrigin>();
        character = GetComponent<CharacterController>();
        if (character == null)
        {
            character = gameObject.AddComponent<CharacterController>();
            character.height = 1.8f;
            character.center = new Vector3(0, 0.9f, 0);
        }
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Transform head = xrOrigin.Camera.transform;

        // 頭の向きに基づいた移動
        Vector3 direction = new Vector3(input.x, 0, input.y);
        Vector3 headYaw = new Vector3(head.forward.x, 0, head.forward.z).normalized;
        Quaternion rotation = Quaternion.LookRotation(headYaw);
        Vector3 move = rotation * direction * moveSpeed * Time.deltaTime;

        character.Move(move);
    }
}