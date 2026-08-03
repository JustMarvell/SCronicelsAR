using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scar.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        readonly List<ISaveable> m_Saveables = new();
        string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Register(ISaveable saveable) => m_Saveables.Add(saveable);
        public void Unregister(ISaveable saveable) => m_Saveables.Remove(saveable);

        public void Save(string chapterId, string sceneName)
        {
            var data = new SaveData { CurrentChapterId = chapterId, CurrentSceneName = sceneName };
            foreach (var s in m_Saveables)
            {
                data.Keys.Add(s.SaveId);
                data.Values.Add(JsonUtility.ToJson(s.CaptureState()));
            }
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        }

        public bool HasSave() => File.Exists(SavePath);

        public SaveData Load()
        {
            if (!HasSave()) return null;
            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            for (int i = 0; i < data.Keys.Count; i++)
            {
                var target = m_Saveables.Find(s => s.SaveId == data.Keys[i]);
                target?.RestoreState(data.Values[i]); // caller's ISaveable deserializes its own JSON string
            }
            return data;
        }
    }
}