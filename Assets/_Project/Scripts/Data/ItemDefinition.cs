using UnityEngine;

namespace Scar.Data
{
    [CreateAssetMenu(menuName = "Scar/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        public string ItemId;
        public string DisplayName;
        public WeaponDefinition EquippableWeapon; // null if not a weapon item
    }
}