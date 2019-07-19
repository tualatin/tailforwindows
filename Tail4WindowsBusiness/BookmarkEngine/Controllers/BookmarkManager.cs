using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using log4net;
using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Args;
using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Delegates;
using Org.Vs.TailForWin.Business.BookmarkEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Utils;


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
    public event BookmarkDataSourceChangedEventHandler OnBookmarkDataSourceChanged;

    #endregion

    #region Properties

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> bookmark data source
    /// </summary>
    public AsyncObservableCollection<LogEntry> BookmarkDataSource
    {
      get;
      private set;
    }

    /// <summary>
    /// Time stamp
    /// </summary>
    public bool TimeStamp
    {
      get;
      set;
    }

    /// <summary>
    /// Bookmark count
    /// </summary>
    public int Count => BookmarkDataSource?.Count(p => p.BookmarkPoint != null) ?? 0;

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BookmarkManager()
    {
      BookmarkDataSource = new AsyncObservableCollection<LogEntry>();
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
    /// <param name="windowId">Window id</param>
    /// <param name="item">Bookmark item</param>
    public void AddBookmarkItemsToSource(Guid windowId, LogEntry item) => AddBookmarkItemsToSource(windowId, new List<LogEntry> { item });

    /// <summary>
    /// Adds bookmark items to data source
    /// </summary>
    /// <param name="windowId">Window id</param>
    /// <param name="itemRange"><see cref="List{T}"/> of bookmarks</param>
    public void AddBookmarkItemsToSource(Guid windowId, List<LogEntry> itemRange)
    {
      if ( !Equals(_activeWindowGuid, windowId) )
        return;

      foreach ( var item in itemRange )
      {
        if ( BookmarkDataSource.Contains(item) )
          continue;

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
    /// Clears the bookmark data source
    /// </summary>
    public void ClearBookmarkDataSource()
    {
      if ( BookmarkDataSource == null || BookmarkDataSource.Count == 0 )
        return;

      BookmarkDataSource.Clear();
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

    private void OnBookmarkDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch ( e.Action )
      {
      case NotifyCollectionChangedAction.Add:

        foreach ( object item in e.NewItems )
        {
          if ( !(item is LogEntry logEntry) )
            continue;

          logEntry.PropertyChanged += OnLogEntryPropertyChanged;
        }
        break;

      case NotifyCollectionChangedAction.Remove:

        foreach ( object item in e.OldItems )
        {
          if ( !(item is LogEntry logEntry) )
            continue;

          logEntry.PropertyChanged -= OnLogEntryPropertyChanged;
        }
        break;

      case NotifyCollectionChangedAction.Replace:

        break;

      case NotifyCollectionChangedAction.Move:

        break;

      case NotifyCollectionChangedAction.Reset:

        break;
      }

      OnBookmarkDataSourceChanged?.Invoke(this, new IdChangedEventArgs(_activeWindowGuid));
    }

    private void OnLogEntryPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is LogEntry logEntry) )
        return;

      if ( logEntry.BookmarkPoint == null || logEntry.IsAutoBookmark )
      {
        OnBookmarkDataSourceChanged?.Invoke(this, new IdChangedEventArgs(_activeWindowGuid));
        return;
      }

      BitmapImage image = BusinessHelper.CreateBitmapIcon(string.IsNullOrWhiteSpace(logEntry.BookmarkToolTip) ?
        "/T4W;component/Resources/Bookmark.png" :
        "/T4W;component/Resources/Bookmark_Info.png");

      logEntry.BookmarkPoint = image;
      OnBookmarkDataSourceChanged?.Invoke(this, new IdChangedEventArgs(_activeWindowGuid));
    }
  }
}
