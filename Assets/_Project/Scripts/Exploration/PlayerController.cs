using UnityEngine;
using UnityEngine.InputSystem;

namespace Scar.Exploration
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] InputActionReference m_MoveAction;
        [SerializeField] Transform m_CameraTransform;
        [SerializeField] float m_MoveSpeed = 5f;
        [SerializeField] float m_TurnSpeed = 10f;
        [SerializeField] float m_Gravity = -9.81f;

        CharacterController m_Controller;
        float m_VerticalVelocity;

        void Awake() => m_Controller = GetComponent<CharacterController>();
        void OnEnable() => m_MoveAction.action.Enable();
        void OnDisable() => m_MoveAction.action.Disable();

        void Update()
        {
            Vector2 input = m_MoveAction.action.ReadValue<Vector2>();
            Vector3 camFwd = m_CameraTransform.forward; camFwd.y = 0; camFwd.Normalize();
            Vector3 camRight = m_CameraTransform.right; camRight.y = 0; camRight.Normalize();
            Vector3 moveDir = camFwd * input.y + camRight * input.x;

            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, m_TurnSpeed * Time.deltaTime);
            }

            m_VerticalVelocity = m_Controller.isGrounded ? -0.5f : m_VerticalVelocity + m_Gravity * Time.deltaTime;
            Vector3 velocity = moveDir * m_MoveSpeed + Vector3.up * m_VerticalVelocity;
            m_Controller.Move(velocity * Time.deltaTime);
        }
    }
}