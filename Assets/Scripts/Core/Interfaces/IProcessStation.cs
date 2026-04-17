namespace YesChef.Core.Interfaces
{
    /// <summary>
    /// Explicit processing contract for stations that transform ingredients over time.
    /// This keeps processing behaviour replaceable without coupling gameplay code to a specific station type.
    /// </summary>
    public interface IProcessStation
    {
        bool IsProcessing { get; }
        float ProgressNormalized { get; }
        bool CanProcess(IHoldable holdable);
        void BeginProcessing(IHoldable holdable);
        void CancelProcessing();
    }
}
