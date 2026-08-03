using UnityEngine;
using Scar.Core;
using Scar.Data;
using Scar.SaveSystem;

namespace Scar.Inventory
{
    public struct WeaponEquippedEvent { public WeaponDefinition Weapon; }

    [System.Serializable] class EquipmentState { public string EquippedWeaponId; }

    public class EquipmentManager : MonoBehaviour, ISaveable
    {
        public static EquipmentManager Instance { get; private set; }
        public string SaveId => "Equipment";

        [SerializeField] WeaponDefinition[] m_AllWeapons; // for id->asset lookup on load

        public WeaponDefinition EquippedWeapon { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SaveManager.Instance.Register(this);
        }

        public void EquipWeapon(WeaponDefinition weapon)
        {
            EquippedWeapon = weapon;
            EventBus.Publish(new WeaponEquippedEvent { Weapon = weapon });
        }

        public object CaptureState() =>
            new EquipmentState { EquippedWeaponId = EquippedWeapon != null ? EquippedWeapon.WeaponId : null };

        public void RestoreState(object state)
        {
            var id = JsonUtility.FromJson<EquipmentState>((string)state).EquippedWeaponId;
            EquippedWeapon = System.Array.Find(m_AllWeapons, w => w.WeaponId == id);
        }
    }
}