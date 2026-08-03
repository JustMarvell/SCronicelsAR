using System.Collections.Generic;
using UnityEngine;
using Scar.Core;
using Scar.SaveSystem;

namespace Scar.Inventory
{
    [System.Serializable] class InventoryState { public List<string> ItemIds = new(); }

    public class InventoryManager : MonoBehaviour, ISaveable
    {
        public static InventoryManager Instance { get; private set; }
        public string SaveId => "Inventory";

        readonly List<string> m_ItemIds = new(); // stores ItemDefinition.ItemId, duplicates allowed for stacks

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SaveManager.Instance.Register(this);
        }

        void OnEnable() => EventBus.Subscribe<Scar.Inventory.ItemCollectedEvent>(OnItemCollected);
        void OnDisable() => EventBus.Unsubscribe<Scar.Inventory.ItemCollectedEvent>(OnItemCollected);

        void OnItemCollected(ItemCollectedEvent evt) => m_ItemIds.Add(evt.ItemId);

        public bool HasItem(string itemId) => m_ItemIds.Contains(itemId);
        public IReadOnlyList<string> Items => m_ItemIds;

        public object CaptureState() => new InventoryState { ItemIds = new List<string>(m_ItemIds) };

        public void RestoreState(object state)
        {
            m_ItemIds.Clear();
            m_ItemIds.AddRange(JsonUtility.FromJson<InventoryState>((string)state).ItemIds);
        }
    }
}