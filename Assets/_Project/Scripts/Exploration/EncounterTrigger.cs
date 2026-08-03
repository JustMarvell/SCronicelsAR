using UnityEngine;
using Scar.Core;

namespace Scar.Exploration
{
    [RequireComponent(typeof(Collider))]
    public class EncounterTrigger : MonoBehaviour
    {
        [SerializeField] string m_EnemyId;
        [SerializeField] string m_WeaponId;
        [SerializeField] bool m_TriggerOnce = true;

        bool m_Triggered;

        void OnTriggerEnter(Collider other)
        {
            if (m_Triggered && m_TriggerOnce) return;
            if (!other.CompareTag("Player")) return;

            m_Triggered = true;
            GameModeController.Instance.RequestModeSwitch(new GameContext
            {
                RequestedMode = GameMode.AR,
                EnemyId = m_EnemyId,
                WeaponId = m_WeaponId
            });
        }
    }
}