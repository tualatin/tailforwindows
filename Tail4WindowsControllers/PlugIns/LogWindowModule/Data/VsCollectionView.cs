using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data
{
  /// <summary>
  /// Virtual Studios collection view
  /// </summary>
  public class VsCollectionView
  {
    #region Properties

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    public ObservableCollection<LogEntry> LogEntries
    {
      get;
      set;
    }

    /// <summary>
    /// Filtered collection <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    public ObservableCollection<LogEntry> FilteredCollection
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsCollectionView()
    {
      LogEntries = new ObservableCollection<LogEntry>();
      FilteredCollection = new ObservableCollection<LogEntry>();
    }
  }
}
