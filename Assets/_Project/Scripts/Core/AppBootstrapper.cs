using System.Collections;
using UnityEngine;

namespace Scar.Core
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] string m_FirstSceneName = "MainMenu";

        IEnumerator Start()
        {
            yield return SceneLoader.Instance.LoadAdditive(m_FirstSceneName);
            GameModeController.Instance.SetInitialScene(m_FirstSceneName);
        }
    }
}