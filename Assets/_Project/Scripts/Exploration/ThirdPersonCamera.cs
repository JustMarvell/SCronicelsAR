using UnityEngine;
using UnityEngine.InputSystem;

namespace Scar.Exploration
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] InputActionReference m_LookAction;
        [SerializeField] Transform m_Target;
        [SerializeField] Vector3 m_Offset = new(0, 2f, -4f);
        [SerializeField] float m_LookSensitivity = 2f;
        [SerializeField] float m_MinPitch = -20f, m_MaxPitch = 60f;

        float m_Yaw, m_Pitch = 15f;

        void OnEnable() => m_LookAction.action.Enable();
        void OnDisable() => m_LookAction.action.Disable();

        void LateUpdate()
        {
            Vector2 look = m_LookAction.action.ReadValue<Vector2>();
            m_Yaw += look.x * m_LookSensitivity;
            m_Pitch = Mathf.Clamp(m_Pitch - look.y * m_LookSensitivity, m_MinPitch, m_MaxPitch);

            Quaternion rot = Quaternion.Euler(m_Pitch, m_Yaw, 0);
            transform.position = m_Target.position + rot * m_Offset;
            transform.LookAt(m_Target.position + Vector3.up * 1.5f);
        }
    }
}