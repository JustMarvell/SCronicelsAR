using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scar.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public IEnumerator LoadAdditive(string sceneName)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded) yield break;
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!op.isDone) yield return null;
        }

        public IEnumerator Unload(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) yield break;
            var op = SceneManager.UnloadSceneAsync(scene);
            while (!op.isDone) yield return null;
        }

        public IEnumerator SwitchScene(string fromScene, string toScene)
        {
            yield return LoadAdditive(toScene);
            if (!string.IsNullOrEmpty(fromScene))
                yield return Unload(fromScene);
        }
    }
}