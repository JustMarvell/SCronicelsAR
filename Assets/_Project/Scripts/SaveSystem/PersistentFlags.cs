using System.Collections.Generic;
using UnityEngine;

namespace Scar.SaveSystem
{
    [System.Serializable] class FlagsState { public List<string> Flags = new(); }

    public class PersistentFlags : MonoBehaviour, ISaveable
    {
        public static PersistentFlags Instance { get; private set; }
        public string SaveId => "PersistentFlags";

        readonly HashSet<string> m_Flags = new();

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SaveManager.Instance.Register(this);
        }

        public bool HasFlag(string id) => m_Flags.Contains(id);
        public void SetFlag(string id) => m_Flags.Add(id);

        public object CaptureState() => new FlagsState { Flags = new List<string>(m_Flags) };

        public void RestoreState(object state)
        {
            m_Flags.Clear();
            foreach (var f in JsonUtility.FromJson<FlagsState>((string)state).Flags)
                m_Flags.Add(f);
        }
    }
}