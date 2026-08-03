using System.Collections;
using UnityEngine;

namespace Scar.Core
{
    public struct GameModeChangedEvent
    {
        public GameMode NewMode;
        public GameContext Context;
    }

    public class GameModeController : MonoBehaviour
    {
        public static GameModeController Instance { get; private set; }

        [SerializeField] string m_ARSceneName = "AR_Session";

        GameMode m_CurrentMode = GameMode.Explore;
        string m_CurrentSceneName;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void SetInitialScene(string sceneName) => m_CurrentSceneName = sceneName;

        public void RequestModeSwitch(GameContext context)
        {
            string target = context.RequestedMode == GameMode.AR ? m_ARSceneName : context.TargetSceneName;
            StartCoroutine(SwitchRoutine(target, context));
        }

        IEnumerator SwitchRoutine(string targetScene, GameContext context)
        {
            yield return SceneLoader.Instance.SwitchScene(m_CurrentSceneName, targetScene);
            m_CurrentSceneName = targetScene;
            m_CurrentMode = context.RequestedMode;
            EventBus.Publish(new GameModeChangedEvent { NewMode = m_CurrentMode, Context = context });
        }
    }
}