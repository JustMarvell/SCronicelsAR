using UnityEngine;

namespace Scar.Data
{
    public enum WeaponType { Melee, Ranged }

    [CreateAssetMenu(menuName = "Scar/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        public string WeaponId;
        public WeaponType Type;
        public float Range = 1.5f;
        public float Cooldown = 1f;
        public int Damage = 10;
    }
}