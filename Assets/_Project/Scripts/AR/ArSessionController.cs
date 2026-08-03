using UnityEngine;
using Scar.Core;

namespace Scar.AR
{
    public class ArSessionController : MonoBehaviour
    {
        [SerializeField] GameObject m_CombatRoot;   // content root, enabled when entering combat
        // [SerializeField] GameObject m_DialogueRoot; // add in later phase
        // [SerializeField] GameObject m_ItemRoot;     // add in later phase

        GameContext m_ActiveContext;

        void OnEnable() => EventBus.Subscribe<GameModeChangedEvent>(OnModeChanged);
        void OnDisable() => EventBus.Unsubscribe<GameModeChangedEvent>(OnModeChanged);

        void OnModeChanged(GameModeChangedEvent evt)
        {
            if (evt.NewMode != GameMode.AR) return;

            m_ActiveContext = evt.Context;
            m_CombatRoot.SetActive(!string.IsNullOrEmpty(m_ActiveContext.EnemyId));
            // future: else if dialogue/item id set, activate that root instead
        }

        public GameContext ActiveContext => m_ActiveContext;
    }
}