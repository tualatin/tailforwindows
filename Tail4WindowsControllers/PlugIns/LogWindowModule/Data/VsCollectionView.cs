using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data
{
  /// <summary>
  /// Virtual Studios collection view
  /// </summary>
  public class VsCollectionView<T> where T : NotifyMaster, new()
  {
    #region Properties

    /// <summary>
    /// <see cref="ObservableCollection{T}"/>
    /// </summary>
    public AsyncObservableCollection<T> Collection
    {
      get;
      set;
    }

    /// <summary>
    /// Filtered collection <see cref="ObservableCollection{T}"/>
    /// </summary>
    public AsyncObservableCollection<T> FilteredCollection
    {
      get;
      set;
    }

    private Func<object, Task<bool>> _filterAsync;

    /// <summary>
    /// Filter async action
    /// </summary>
    public Func<object, Task<bool>> FilterAsync
    {
      get => _filterAsync;
      set
      {
        if ( _filterAsync != null && value == _filterAsync )
          return;

        _filterAsync = value;
      }
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
      if ( list != null )
      {
        IEnumerable<T> enumerable = list as T[] ?? list.ToArray();
        Collection = new AsyncObservableCollection<T>(enumerable);
        FilteredCollection = new AsyncObservableCollection<T>(enumerable);
      }
      else
      {
        Collection = new AsyncObservableCollection<T>();
        FilteredCollection = new AsyncObservableCollection<T>();
      }

      //Collection.CollectionChanged += OnLogEntriesCollectionChanged;
    }

    private async void OnLogEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch ( e.Action )
      {
      case NotifyCollectionChangedAction.Add:

        foreach ( T item in e.NewItems )
        {
          if ( FilterAsync == null )
          {
            FilteredCollection.Add(item);
          }
          else
          {
            bool result = await FilterAsync?.Invoke(item);

            if ( result )
            {
              FilteredCollection.Add(item);
            }
          }
        }
        break;

      case NotifyCollectionChangedAction.Remove:

        break;

      case NotifyCollectionChangedAction.Replace:

        break;

      case NotifyCollectionChangedAction.Move:

        break;

      case NotifyCollectionChangedAction.Reset:

        FilteredCollection?.Clear();
        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }

    /// <summary>
    /// Clears collections
    /// </summary>
    public void Clear() => Collection.Clear();

    /// <summary>
    /// Clears filtered collection
    /// </summary>
    public void ClearFilteredCollection() => FilteredCollection.Clear();

    /// <summary>
    /// Release all resources used by <see cref="VsCollectionView{T}"/>
    /// </summary>
    public void Dispose()
    {
      Collection.Clear();
      ClearFilteredCollection();

      Collection = null;
      FilteredCollection = null;

      GC.SuppressFinalize(this);
    }
  }
}
