using UnityEngine;

using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

// Example of using an InputActionMap named "Player" from a UnityEngine.MonoBehaviour implementing callback interface.
 public class VRKeyBoardMoveController : MonoBehaviour
 {
    public InputActionProperty moveAction; // assgin Key Board Input.
    public InputActionProperty rotationAction;
    [SerializeField] XROrigin xrOrigin;
    [SerializeField] private CharacterController character;
    public float moveSpeed = 1.5f;
    public float turnSpeed = 60f; // 度/秒
    private Vector3 movingVec;
    private Vector3 roationVec;

   

    void Start()
    {
        if (character == null)
        {
            character = gameObject.AddComponent<CharacterController>();
            character.height = 1.8f;
            character.center = new Vector3(0, 0.9f, 0);
        }
    }
    private void OnEnable()
    {
        EnableAction(true);
    }

    private void OnDisable()
    {
        EnableAction(false);
    }
    void Update()
    {
        if (moveAction.action.ReadValue<Vector2>() == Vector2.zero)
        {
            movingVec = Vector3.zero;
        }
        character.Move(movingVec);
        if(rotationAction.action.ReadValue<Vector2>() == Vector2.zero)
        {
            roationVec = Vector3.zero;
        }
        xrOrigin.gameObject.transform.Rotate(roationVec);
    }
    private void EnableAction (bool enable)
    {
        if (moveAction.action == null) return;
        if (enable)
        {
            moveAction.action.performed += OnMove;
            moveAction.action.Enable();
        }
        else
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.Disable();
        }
        if(rotationAction.action == null) return;
        if (enable)
        {
            rotationAction.action.performed += OnRotation;
            rotationAction.action.Enable();
        }
        else
        {
            rotationAction.action.performed -= OnRotation;
            rotationAction.action.Disable();
        }
    }
    private void OnMove(InputAction.CallbackContext content)
    {
        Vector2 input = content.ReadValue<Vector2>();
        Transform head = xrOrigin.Camera.transform;

        // 頭の向きに基づいた移動
        Vector3 direction = new Vector3(input.x, 0, input.y);
        Vector3 headYaw = new Vector3(head.forward.x, 0, head.forward.z).normalized;
        Quaternion rotation = Quaternion.LookRotation(headYaw);
        movingVec = rotation * direction * moveSpeed * Time.deltaTime;
    }
    private void OnRotation(InputAction.CallbackContext content)
    {
        Vector2 input = content.action.ReadValue<Vector2>();
        Vector3 direction = new Vector3(-input.y,input.x,0);
        roationVec = direction * turnSpeed * Time.deltaTime;
    }
}