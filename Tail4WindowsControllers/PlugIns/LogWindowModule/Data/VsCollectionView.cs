using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data
{
  /// <summary>
  /// Virtual Studios collection view
  /// </summary>
  public class VsCollectionView<T> : NotifyMaster, IVsCollectionView<T>
  {
    private SemaphoreSlim _semaphore;
    private HashSet<object> _filteredCollectionHashSet;
    private bool _isFilterNotNull;

    #region Properties

    /// <summary>
    /// <see cref="ObservableCollection{T}"/>
    /// </summary>
    public AsyncObservableCollection<T> Collection
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsCollectionView() => Initialize(null);

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="list"><see cref="IEnumerable{T}"/></param>
    public VsCollectionView(IEnumerable<T> list) => Initialize(list);

    private void Initialize(IEnumerable<T> list)
    {
      _semaphore = new SemaphoreSlim(1, 1);

      if ( list != null )
      {
        IEnumerable<T> enumerable = list as T[] ?? list.ToArray();
        Collection = new AsyncObservableCollection<T>(enumerable);
      }
      else
      {
        Collection = new AsyncObservableCollection<T>();
      }
    }

    /// <summary>
    /// Event Filter started
    /// </summary>
    public event EventHandler<EventArgs> FilteringStarted;

    /// <summary>
    /// Event Filter finished
    /// </summary>
    public event EventHandler<EventArgs> FilteringFinished;

    /// <summary>
    /// Event Filter error
    /// </summary>
    public event EventHandler<FilteringEventArgs> FilteringError;

    /// <summary>
    /// OnFilteringStarted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilteringStarted(object sender, EventArgs e)
    {
      EventHandler<EventArgs> localEvent = FilteringStarted;
      localEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// OnFilteringFinished
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilteringFinished(object sender, EventArgs e)
    {
      EventHandler<EventArgs> localEvent = FilteringFinished;
      localEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// OnFilteringError
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilteringError(object sender, FilteringEventArgs e)
    {
      EventHandler<FilteringEventArgs> localEvent = FilteringError;
      localEvent?.Invoke(sender, e);
    }

    private void SetFilter(ICollectionView view, Predicate<object> filter)
    {
      if ( view == null || !view.CanFilter )
      {
        return;
      }

      view.Filter = filter;
      _isFilterNotNull = filter != null;
    }

    private void ApplyFilter()
    {
      ExecuteFilterAction(() =>
      {
        var view = CollectionViewSource.GetDefaultView();

        if ( view != null )
        {
          if ( _filteredCollectionHashSet != null )
          {
            SetFilter(view, ItemPassesFilter);
          }
          else
          {
            SetFilter(view, null);
          }
        }

        OnFilteringFinished(this, EventArgs.Empty);
      });
    }

    /// <summary>
    /// ItemPassesFilter
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool ItemPassesFilter(object item) => _filteredCollectionHashSet.Contains(item);

    private void ExecuteFilterAction(Action action)
    {
      var worker = new BackgroundWorker();
      worker.DoWork += (s, e) =>
      {
        _semaphore.Wait();
        action?.Invoke();
      };

      worker.RunWorkerCompleted += (s, e) =>
      {
        if ( e.Error != null )
        {
          // TODO error handling
        }

        _semaphore.Release();
      };

      worker.RunWorkerAsync();
    }

    /// <summary>
    /// Clears collections
    /// </summary>
    public void Clear() => Collection.Clear();

    /// <summary>
    /// Release all resources used by <see cref="VsCollectionView{T}"/>
    /// </summary>
    public void Dispose()
    {
      Collection.Clear();
      Collection = null;

      GC.SuppressFinalize(this);
    }
  }
}
