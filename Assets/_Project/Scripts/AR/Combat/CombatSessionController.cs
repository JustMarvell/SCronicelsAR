using UnityEngine;
using Scar.Data;

namespace Scar.AR.Combat
{
    public class CombatSessionController : MonoBehaviour
    {
        [SerializeField] WeaponDefinition m_TestWeapon; // TODO: replace with EquipmentManager query (Phase 7)
        [SerializeField] EnemyCombatant m_TestEnemy;    // TODO: resolve via GameContext.EnemyId
        [SerializeField] MeleeWeaponController m_MeleeController;
        [SerializeField] RangedWeaponController m_RangedController;

        IWeaponController m_Active;

        void OnEnable()
        {
            m_Active = m_TestWeapon.Type == WeaponType.Melee
                ? m_MeleeController
                : (IWeaponController)m_RangedController;

            ((MonoBehaviour)m_Active).gameObject.SetActive(true);
            m_Active.EnterCombat(m_TestEnemy, m_TestWeapon);
        }

        void Update() => m_Active?.Tick();
        void OnDisable() => m_Active?.ExitCombat();
    }
}