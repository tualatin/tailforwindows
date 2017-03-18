using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// SmartWatch object
  /// </summary>
  public class SmartWatchData : INotifyMaster
  {
    private bool filterByExtension;

    /// <summary>
    /// Filter new files by extension
    /// </summary>
    public bool FilterByExtension
    {
      get => filterByExtension;
      set
      {
        filterByExtension = value;
        OnPropertyChanged("FilterByExtension");
      }
    }

    private bool saveLastDecision;

    /// <summary>
    /// Save last decision
    /// </summary>
    public bool SaveLastDecision
    {
      get => saveLastDecision;
      set
      {
        saveLastDecision = value;
        OnPropertyChanged("SaveLastDecision");
      }
    }
  }
}
