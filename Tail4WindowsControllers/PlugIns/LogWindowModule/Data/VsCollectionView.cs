using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data
{
  /// <summary>
  /// Virtual Studios collection view
  /// </summary>
  public class VsCollectionView<T> where T : NotifyMaster, new()
  {
    private readonly Func<T, TailData, Task> _filterFunction;

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

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsCollectionView() => Initialize();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="filterFunction">Filter method async</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    public VsCollectionView(Func<T, TailData, Task> filterFunction, TailData tailData)
    {
      _filterFunction = filterFunction;
      CurrentTailData = tailData;

      Initialize();
    }

    private void Initialize()
    {
      Collection = new AsyncObservableCollection<T>();
      FilteredCollection = new AsyncObservableCollection<T>();

      Collection.CollectionChanged += OnLogEntriesCollectionChanged;
    }

    private void OnLogEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        FilteredCollection.Clear();
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
      Clear();

      Collection = null;
      FilteredCollection = null;
    }
  }
}
