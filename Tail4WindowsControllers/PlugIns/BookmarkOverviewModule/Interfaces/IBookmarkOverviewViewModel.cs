using System.Collections;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Interfaces
{
  /// <summary>
  /// Bookmark overview model interface
  /// </summary>
  public interface IBookmarkOverviewViewModel
  {
    /// <summary>
    /// Loaded command
    /// </summary>
    ICommand LoadedCommand
    {
      get;
    }

    /// <summary>
    /// Closing command
    /// </summary>
    ICommand ClosingCommand
    {
      get;
    }

    /// <summary>
    /// Export command
    /// </summary>
    IAsyncCommand ExportCommand
    {
      get;
    }

    /// <summary>
    /// BookmarkOverview mouse double click command
    /// </summary>
    ICommand BookmarkOverviewMouseDoubleClickCommand
    {
      get;
    }

    /// <summary>
    /// Remove bookmarks command
    /// </summary>
    ICommand RemoveBookmarksCommand
    {
      get;
    }

    /// <summary>
    /// Add bookmark comment command
    /// </summary>
    ICommand AddBookmarkCommentCommand
    {
      get;
    }

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
    /// SelectedItems
    /// </summary>
    IList SelectedItems
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
    /// Setup Bookmark collection view
    /// </summary>
    void SetupBookmarkCollectionView();
  }
}
