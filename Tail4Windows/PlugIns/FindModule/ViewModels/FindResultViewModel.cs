using System.Collections.ObjectModel;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.PlugIns.FindModule.Controller;


namespace Org.Vs.TailForWin.PlugIns.FindModule.ViewModels
{
  /// <summary>
  /// FindResult view model
  /// </summary>
  public class FindResultViewModel : NotifyMaster
  {
    #region Properties

    /// <summary>
    /// FindResult view
    /// </summary>
    public ListCollectionView FindResultCollectionView
    {
      get;
      set;
    }

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    private ObservableCollection<LogEntry> FindResultSource
    {
      get;
      set;
    } = new ObservableCollection<LogEntry>();

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindResultViewModel()
    {
      SetupFindResultCollecitonView();
    }

    private void SetupFindResultCollecitonView()
    {
      FindResultCollectionView = (ListCollectionView) new CollectionViewSource { Source = FindResultSource }.View;
      FindResultCollectionView.CustomSort = new LogEntryComparer();
    }
  }
}
