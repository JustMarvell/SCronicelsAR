using UnityEngine;
using Scar.Core;

namespace Scar.AR.Combat
{
    public struct EnemyDefeatedEvent { public EnemyCombatant Enemy; }

    public class EnemyCombatant : MonoBehaviour
    {
        [SerializeField] int m_MaxHealth = 50;
        int m_CurrentHealth;

        void Awake() => m_CurrentHealth = m_MaxHealth;
        public bool IsDead => m_CurrentHealth <= 0;

        public void TakeDamage(int amount)
        {
            if (IsDead) return;
            m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - amount);
            if (IsDead) EventBus.Publish(new EnemyDefeatedEvent { Enemy = this });
        }
    }
}