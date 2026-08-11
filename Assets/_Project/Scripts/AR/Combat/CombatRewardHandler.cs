using UnityEngine;
using Scar.Core;

namespace Scar.AR.Combat
{
    public struct RewardGrantedEvent { public string EnemyId; } // hook for Inventory (Phase 7)

    public class CombatRewardHandler : MonoBehaviour
    {
        [SerializeField] ArSessionController m_ArSession;
        [SerializeField] float m_ReturnDelay = 1f; // brief pause for reward UI/feedback

        void OnEnable() => EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
        void OnDisable() => EventBus.Unsubscribe<EnemyDefeatedEvent>(OnEnemyDefeated);

        void OnEnemyDefeated(EnemyDefeatedEvent evt)
        {
            var ctx = m_ArSession.ActiveContext;
            EventBus.Publish(new RewardGrantedEvent { EnemyId = ctx.EnemyId });
            Invoke(nameof(ReturnToExplore), m_ReturnDelay);
        }

        void ReturnToExplore()
        {
            var ctx = m_ArSession.ActiveContext;
            Scar.SaveSystem.PersistentFlags.Instance.SetFlag($"encounter_{ctx.EnemyId}");
            GameModeController.Instance.RequestModeSwitch(new GameContext
            {
                RequestedMode = GameMode.Explore,
                TargetSceneName = ctx.TargetSceneName,
                HasReturnPosition = ctx.HasReturnPosition,
                ReturnPosition = ctx.ReturnPosition,
                ReturnRotation = ctx.ReturnRotation
            });
        }
    }
}