using UnityEngine;
using UnityEngine.InputSystem;
using Scar.Data;

namespace Scar.AR.Combat
{
    public class RangedWeaponController : MonoBehaviour, IWeaponController
    {
        [SerializeField] InputActionReference m_PressAction;    // Button, started/canceled
        [SerializeField] InputActionReference m_PointerPosition; // Vector2
        [SerializeField] Camera m_ARCamera;
        [SerializeField] LayerMask m_EnemyLayer;

        EnemyCombatant m_Target;
        WeaponDefinition m_Weapon;
        float m_NextAttackTime;
        bool m_IsAiming;

        public void EnterCombat(EnemyCombatant target, WeaponDefinition weapon)
        {
            m_Target = target;
            m_Weapon = weapon;
            m_PressAction.action.started += OnPressStart;
            m_PressAction.action.canceled += OnRelease;
            m_PressAction.action.Enable();
            m_PointerPosition.action.Enable();
        }

        public void ExitCombat()
        {
            m_PressAction.action.started -= OnPressStart;
            m_PressAction.action.canceled -= OnRelease;
            m_PressAction.action.Disable();
            m_PointerPosition.action.Disable();
            m_IsAiming = false;
            m_Target = null;
        }

        public void Tick() { } // reticle UI update goes here, if/when added

        void OnPressStart(InputAction.CallbackContext ctx)
        {
            if (m_Target == null || m_Target.IsDead || Time.time < m_NextAttackTime) return;
            m_IsAiming = true;
        }

        void OnRelease(InputAction.CallbackContext ctx)
        {
            if (!m_IsAiming) return;
            m_IsAiming = false;

            Vector2 screenPos = m_PointerPosition.action.ReadValue<Vector2>();
            Ray ray = m_ARCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 100f, m_EnemyLayer) &&
                hit.collider.GetComponentInParent<EnemyCombatant>() == m_Target)
            {
                m_Target.TakeDamage(m_Weapon.Damage);
            }
            m_NextAttackTime = Time.time + m_Weapon.Cooldown;
        }
    }
}