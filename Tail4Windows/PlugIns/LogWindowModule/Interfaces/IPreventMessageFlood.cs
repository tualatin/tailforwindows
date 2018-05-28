namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// Prevent message flood interface
  /// </summary>
  public interface IPreventMessageFlood
  {
    /// <summary>
    /// IsBusy indicator
    /// </summary>
    bool IsBusy
    {
      get;
    }

    /// <summary>
    /// Updates busy state
    /// </summary>
    void UpdateBusyState();
  }
}
