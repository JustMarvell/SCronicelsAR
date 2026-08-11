using UnityEngine;
using Scar.Inventory;
using Scar.Data;

namespace Scar.Testing
{
    public class TestAutoEquip : MonoBehaviour
    {
        [SerializeField] WeaponDefinition m_Weapon;
        void Start() => EquipmentManager.Instance.EquipWeapon(m_Weapon);
    }
}