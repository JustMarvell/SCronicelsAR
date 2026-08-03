namespace Scar.SaveSystem
{
    public interface ISaveable
    {
        string SaveId { get; }           // stable unique key, e.g. "PlayerPosition"
        object CaptureState();
        void RestoreState(object state);
    }
}