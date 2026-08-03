using Scar.Data;

namespace Scar.AR.Combat
{
    public interface IWeaponController
    {
        void EnterCombat(EnemyCombatant target, WeaponDefinition weapon);
        void Tick();
        void ExitCombat();
    }
}