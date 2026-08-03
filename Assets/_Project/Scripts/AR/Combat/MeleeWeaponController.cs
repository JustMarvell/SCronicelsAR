using UnityEngine;
using UnityEngine.InputSystem;
using Scar.Data;

namespace Scar.AR.Combat
{
    public class MeleeWeaponController : MonoBehaviour, IWeaponController
    {
        [SerializeField] InputActionReference m_AttackAction;   // Button
        [SerializeField] InputActionReference m_PointerPosition; // Vector2
        [SerializeField] Camera m_ARCamera;
        [SerializeField] LayerMask m_EnemyLayer;

        EnemyCombatant m_Target;
        WeaponDefinition m_Weapon;
        float m_NextAttackTime;

        public void EnterCombat(EnemyCombatant target, WeaponDefinition weapon)
        {
            m_Target = target;
            m_Weapon = weapon;
            m_AttackAction.action.performed += OnAttack;
            m_AttackAction.action.Enable();
            m_PointerPosition.action.Enable();
        }

        public void ExitCombat()
        {
            m_AttackAction.action.performed -= OnAttack;
            m_AttackAction.action.Disable();
            m_PointerPosition.action.Disable();
            m_Target = null;
        }

        public void Tick() { }

        void OnAttack(InputAction.CallbackContext ctx)
        {
            if (m_Target == null || m_Target.IsDead || Time.time < m_NextAttackTime) return;

            Vector2 screenPos = m_PointerPosition.action.ReadValue<Vector2>();
            Ray ray = m_ARCamera.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit, 100f, m_EnemyLayer)) return;
            if (hit.collider.GetComponentInParent<EnemyCombatant>() != m_Target) return;

            float dist = Vector3.Distance(transform.position, m_Target.transform.position);
            if (dist > m_Weapon.Range) return; // TODO: "move closer" UI cue

            m_Target.TakeDamage(m_Weapon.Damage);
            m_NextAttackTime = Time.time + m_Weapon.Cooldown;
        }
    }
}