using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Delegates;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Business.BookmarkEngine.Interfaces
{
  /// <summary>
  /// Bookmark manager interface
  /// </summary>
  public interface IBookmarkManager
  {
    #region Events

    /// <summary>
    /// On Window Id changed event
    /// </summary>
    event IdChangedEventHandler OnIdChanged;

    /// <summary>
    /// On Bookmark data source changed event
    /// </summary>
    event EventHandler OnBookmarkDataSourceChanged;

    #endregion

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> bookmark data source
    /// </summary>
    ObservableCollection<LogEntry> BookmarkDataSource
    {
      get;
    }

    /// <summary>
    /// Time stamp
    /// </summary>
    bool TimeStamp
    {
      get;
      set;
    }

    /// <summary>
    /// Gets current <see cref="Guid"/>
    /// </summary>
    /// <returns>Current Window id</returns>
    Guid GetCurrentWindowId();

    /// <summary>
    /// Register a <see cref="Guid"/>
    /// </summary>
    /// <param name="windowId">Window id</param>
    /// <param name="activated">Current window is activated again</param>
    void RegisterWindowId(Guid windowId, bool activated = false);

    /// <summary>
    /// Adds a bookmark item to data source
    /// </summary>
    /// <param name="item">Bookmark item</param>
    void AddBookmarkItemsToSource(LogEntry item);

    /// <summary>
    /// Adds bookmark items to data source
    /// </summary>
    /// <param name="itemRange"><see cref="List{T}"/> of bookmarks</param>
    void AddBookmarkItemsToSource(List<LogEntry> itemRange);

    /// <summary>
    /// Removes item from bookmark data source
    /// </summary>
    /// <param name="item">Item to remove</param>
    void RemoveFromBookmarkDataSource(LogEntry item);

    /// <summary>
    /// Release all used resources
    /// </summary>
    void Dispose();
  }
}
