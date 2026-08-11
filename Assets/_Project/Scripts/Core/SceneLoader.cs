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
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
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

        public IEnumerator UnloadAllExcept(params string[] keepNames)
        {
            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (System.Array.IndexOf(keepNames, scene.name) < 0)
                    yield return SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}