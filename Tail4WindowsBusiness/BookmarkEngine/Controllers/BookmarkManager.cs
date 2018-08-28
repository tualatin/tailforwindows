using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using log4net;
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
    public void RegisterWindowId(Guid windowId)
    {
      _activeWindowGuid = windowId;
      LOG.Debug($"Current activated window id is {_activeWindowGuid}");

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

        break;

      case NotifyCollectionChangedAction.Remove:

        break;

      case NotifyCollectionChangedAction.Replace:

        break;

      case NotifyCollectionChangedAction.Move:

        break;

      case NotifyCollectionChangedAction.Reset:

        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }
  }
}
