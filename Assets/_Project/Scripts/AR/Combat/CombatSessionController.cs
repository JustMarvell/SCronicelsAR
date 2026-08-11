using UnityEngine;
using Scar.Data;

namespace Scar.AR.Combat
{
    public class CombatSessionController : MonoBehaviour
    {
        [SerializeField] EnemyCombatant m_TestEnemy;    // TODO: resolve via GameContext.EnemyId
        [SerializeField] MeleeWeaponController m_MeleeController;
        [SerializeField] RangedWeaponController m_RangedController;

        IWeaponController m_Active;

        void OnEnable()
        {
            var weapon = Scar.Inventory.EquipmentManager.Instance.EquippedWeapon;
            if (weapon == null) { Debug.LogWarning("No weapon equipped."); enabled = false; return; }

            m_Active = weapon.Type == WeaponType.Melee
                ? m_MeleeController
                : (IWeaponController)m_RangedController;

            ((MonoBehaviour)m_Active).gameObject.SetActive(true);
            m_Active.EnterCombat(m_TestEnemy, weapon);
        }

        void Update() => m_Active?.Tick();
        void OnDisable() => m_Active?.ExitCombat();
    }
}