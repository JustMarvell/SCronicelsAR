using UnityEngine;
using Scar.Core;

namespace Scar.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] GameObject m_MainPanel;
        [SerializeField] GameObject m_LevelSelectPanel;

        public void OnPlayPressed()
        {
            m_MainPanel.SetActive(false);
            m_LevelSelectPanel.SetActive(true);
        }

        public void OnBackPressed()
        {
            m_LevelSelectPanel.SetActive(false);
            m_MainPanel.SetActive(true);
        }

        public void OnLevelSelected(string sceneName)
        {
            GameModeController.Instance.RequestModeSwitch(new GameContext
            {
                RequestedMode = GameMode.Explore,
                TargetSceneName = sceneName
            });
        }
    }
}