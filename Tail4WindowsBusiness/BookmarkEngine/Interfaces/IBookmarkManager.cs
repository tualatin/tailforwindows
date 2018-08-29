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

    #endregion

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> bookmark data source
    /// </summary>
    ObservableCollection<LogEntry> BookmarkDataSource
    {
      get;
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
    void RegisterWindowId(Guid windowId);

    /// <summary>
    /// Adds bookmark items to data source
    /// </summary>
    /// <param name="itemRange"><see cref="List{T}"/> of bookmarks</param>
    void AddBookmarkItemsToSource(List<LogEntry> itemRange);

    /// <summary>
    /// Release all used resources
    /// </summary>
    void Dispose();
  }
}
