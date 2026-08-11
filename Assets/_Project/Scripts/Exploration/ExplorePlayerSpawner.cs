using UnityEngine;
using Scar.Core;

namespace Scar.Exploration
{
    [RequireComponent(typeof(CharacterController))]
    public class ExplorePlayerSpawner : MonoBehaviour
    {
        CharacterController m_Controller;
        void Awake() => m_Controller = GetComponent<CharacterController>();
        void OnEnable() => EventBus.Subscribe<GameModeChangedEvent>(OnModeChanged);
        void OnDisable() => EventBus.Unsubscribe<GameModeChangedEvent>(OnModeChanged);

        void OnModeChanged(GameModeChangedEvent evt)
        {
            if (evt.NewMode != GameMode.Explore || !evt.Context.HasReturnPosition) return;
            m_Controller.enabled = false;
            transform.SetPositionAndRotation(evt.Context.ReturnPosition, evt.Context.ReturnRotation);
            m_Controller.enabled = true;
        }
    }
}