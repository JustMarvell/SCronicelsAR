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

        void Start()
        {
            if (Scar.SaveSystem.PersistentFlags.Instance.HasFlag($"encounter_{m_EnemyId}"))
                gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (m_Triggered && m_TriggerOnce) return;
            if (!other.CompareTag("Player")) return;

            m_Triggered = true;
            GameModeController.Instance.RequestModeSwitch(new GameContext
            {
                RequestedMode = GameMode.AR,
                EnemyId = m_EnemyId,
                WeaponId = m_WeaponId,
                TargetSceneName = GameModeController.Instance.CurrentSceneName,
                HasReturnPosition = true,
                ReturnPosition = other.transform.position,
                ReturnRotation = other.transform.rotation
            });
        }
    }
}