using UnityEngine;
using Scar.Core;
using Scar.SaveSystem;

namespace Scar.Inventory
{
    public struct ItemCollectedEvent { public string ItemId; }

    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] string m_PickupId;
        [SerializeField] Scar.Data.ItemDefinition m_Item;

        void Start()
        {
            if (PersistentFlags.Instance.HasFlag(m_PickupId)) gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            PersistentFlags.Instance.SetFlag(m_PickupId);
            EventBus.Publish(new ItemCollectedEvent { ItemId = m_Item.ItemId });
            gameObject.SetActive(false);
        }
    }
}