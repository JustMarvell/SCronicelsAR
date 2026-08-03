using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scar.SaveSystem
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void SaveCheckpoint(string chapterId) =>
            SaveManager.Instance.Save(chapterId, SceneManager.GetActiveScene().name);

        public bool TryLoadCheckpoint(out SaveData data)
        {
            data = SaveManager.Instance.Load();
            return data != null;
        }
    }
}