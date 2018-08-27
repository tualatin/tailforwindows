using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Interfaces
{
  /// <summary>
  /// Bookmark overview model interface
  /// </summary>
  public interface IBookmarkOverviewViewModel : IViewModelBase
  {
    /// <summary>
    /// Top position
    /// </summary>
    double TopPosition
    {
      get;
      set;
    }

    /// <summary>
    /// Left position
    /// </summary>
    double LeftPosition
    {
      get;
      set;
    }

    /// <summary>
    /// Window height
    /// </summary>
    double WindowHeight
    {
      get;
      set;
    }

    /// <summary>
    /// Window width
    /// </summary>
    double WindowWidth

    {
      get;
      set;
    }

    /// <summary>
    /// Bookmark view
    /// </summary>
    ListCollectionView BookmarkCollectionView
    {
      get;
    }

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    ObservableCollection<LogEntry> BookmarkSource
    {
      get;
      set;
    }

    /// <summary>
    /// SelectedItems
    /// </summary>
    ObservableCollection<LogEntry> SelectedItems
    {
      get;
    }

    /// <summary>
    /// Selected item
    /// </summary>
    LogEntry SelectedItem
    {
      get;
    }

    /// <summary>
    /// Current filter text
    /// </summary>
    string FilterText
    {
      get;
    }

    /// <summary>
    /// Filter has focus
    /// </summary>
    bool FilterHasFocus
    {
      get;
    }

    /// <summary>
    /// Which window calls the find dialog
    /// </summary>
    Guid WindowGuid
    {
      get;
      set;
    }
  }
}
