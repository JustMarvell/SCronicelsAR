using System;
using System.Collections.Generic;

namespace Scar.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public string CurrentChapterId;
        public string CurrentSceneName;
        public List<string> Keys = new();
        public List<string> Values = new(); // JSON-serialized per-entry state
    }
}