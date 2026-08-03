using Scar.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scar.Testing
{
    public class TestBootstraper : MonoBehaviour
    {
        public string testSceneName;
        void Start() => GameModeController.Instance.SetInitialScene(testSceneName);
    }
}