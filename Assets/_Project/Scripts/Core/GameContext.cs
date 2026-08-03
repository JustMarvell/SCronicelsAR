namespace Scar.Core
{
    public enum GameMode { Explore, AR }

    [System.Serializable]
    public class GameContext
    {
        public GameMode RequestedMode;
        public string TargetSceneName;  // explore scene to load, when RequestedMode == Explore
        public string EnemyId;          // populated for combat encounters
        public string WeaponId;
    }
}