﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using log4net;
using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Args;
using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Delegates;
using Org.Vs.TailForWin.Business.BookmarkEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Business.BookmarkEngine.Controllers
{
  /// <summary>
  /// Bookmark manager
  /// </summary>
  public class BookmarkManager : IBookmarkManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(BookmarkManager));

    private Guid _activeWindowGuid;

    #region Events

    /// <summary>
    /// On Window Id changed event
    /// </summary>
    public event IdChangedEventHandler OnIdChanged;

    /// <summary>
    /// On Bookmark data source changed event
    /// </summary>
    public event EventHandler OnBookmarkDataSourceChanged;

    #endregion

    #region Properties

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> bookmark data source
    /// </summary>
    public ObservableCollection<LogEntry> BookmarkDataSource
    {
      get;
      private set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BookmarkManager()
    {
      BookmarkDataSource = new ObservableCollection<LogEntry>();
      BookmarkDataSource.CollectionChanged += OnBookmarkDataSourceCollectionChanged;
    }

    /// <summary>
    /// Gets current <see cref="Guid"/>
    /// </summary>
    /// <returns>Current Window id</returns>
    public Guid GetCurrentWindowId() => _activeWindowGuid;

    /// <summary>
    /// Register a <see cref="Guid"/>
    /// </summary>
    /// <param name="windowId">Window id</param>
    /// <param name="activated">Current window is activated again</param>
    public void RegisterWindowId(Guid windowId, bool activated = false)
    {
      // Already exists...
      if ( Equals(_activeWindowGuid, windowId) )
        return;

      _activeWindowGuid = windowId;
      LOG.Debug($"Current activated window id is {_activeWindowGuid}, activated: {activated}");

      BookmarkDataSource.Clear();

      if ( activated )
        OnIdChanged?.Invoke(this, new IdChangedEventArgs(_activeWindowGuid));
    }

    /// <summary>
    /// Adds a bookmark item to data source
    /// </summary>
    /// <param name="item">Bookmark item</param>
    public void AddBookmarkItemsToSource(LogEntry item) => AddBookmarkItemsToSource(new List<LogEntry> { item });

    /// <summary>
    /// Adds bookmark items to data source
    /// </summary>
    /// <param name="itemRange"><see cref="List{T}"/> of bookmarks</param>
    public void AddBookmarkItemsToSource(List<LogEntry> itemRange)
    {
      foreach ( LogEntry item in itemRange )
      {
        BookmarkDataSource.Add(item);
      }
    }

    /// <summary>
    /// Removes item from bookmark data source
    /// </summary>
    /// <param name="item">Item to remove</param>
    public void RemoveFromBookmarkDataSource(LogEntry item)
    {
      if ( BookmarkDataSource == null || BookmarkDataSource.Count == 0 )
        return;

      if ( !BookmarkDataSource.Contains(item) )
        return;

      BookmarkDataSource.Remove(item);
    }

    /// <summary>
    /// Release all used resources
    /// </summary>
    public void Dispose()
    {
      BookmarkDataSource.Clear();
      BookmarkDataSource = null;
      _activeWindowGuid = Guid.Empty;
    }

    private void OnBookmarkDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnBookmarkDataSourceChanged?.Invoke(this, EventArgs.Empty);
  }
}
