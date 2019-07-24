using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Collections.FilterCollections
{
  /// <summary>
  /// Virtual Studio Collection view
  /// </summary>
  /// <typeparam name="T">Type of Collection view</typeparam>
  [CollectionDataContract]
  public class VsCollectionView<T> : IVsCollectionView<T>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(VsCollectionView<T>));

    /// <summary>
    /// Max <see cref="SemaphoreSlim"/> count
    /// </summary>
    private const int MaxLockCount = 10;

    /// <summary>
    /// Paging size
    /// </summary>
    private const int PagingSize = 1000;

    private readonly CancellationTokenSource _cts;
    private readonly SemaphoreSlim _semaphoreLock;
    private readonly SemaphoreSlim _semaphoreEstablishQueueLock;
    private ConcurrentQueue<T> _collectionQueue;
    private HashSet<T> _internalCollection;
    private bool _isFilteringStarted;

    /// <summary>
    /// Fires, when the filtering is completed
    /// </summary>
    public event EventHandler<FilterEventArgs> FilteringCompleted;

    /// <summary>
    /// Fires, when an error occurred while filtering
    /// </summary>
    public event EventHandler<FilterEventArgs> FilteringErrorOccurred;

    /// <summary>
    /// Fires, when the filtering is startet
    /// </summary>
    public event EventHandler<EventArgs> FilteringStarted;

    /// <summary>
    /// Collection view lock
    /// </summary>
    public SemaphoreSlim CollectionViewLock
    {
      get;
    } = new SemaphoreSlim(1, MaxLockCount);

    /// <summary>
    /// Current filtered items
    /// </summary>
    public ObservableCollection<T> Items => FilteredCollection.Items;

    /// <summary>
    /// Current items count
    /// </summary>
    public int Count => FilteredCollection.Count;

    private Func<object, Task<bool>> _filter;

    /// <summary>
    /// Async filter function
    /// </summary>
    public Func<object, Task<bool>> Filter
    {
      get => _filter;
      set
      {
        _filter = value;

        if ( FilteredCollection == null )
          FilteredCollection = new AsyncObservableCollection<T>();

        if ( _filter != null )
        {
          _isFilteringStarted = true;
          FilteringStarted?.Invoke(this, EventArgs.Empty);
          LOG.Debug("Enabling filtering...");
        }

        FilteredCollection.Clear();
        NotifyTaskCompletion.Create(EstablishQueueAsync(Collection));
      }
    }

    /// <summary>
    /// Get value by key index
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>The corresponding value of given key</returns>
    public T this[int key]
    {
      get => FilteredCollection[key];
      set => FilteredCollection[key] = value;
    }

    /// <summary>
    /// Complete collection
    /// </summary>
    private AsyncObservableCollection<T> Collection
    {
      get;
      set;
    }

    /// <summary>
    /// Filtered collection
    /// </summary>
    private AsyncObservableCollection<T> FilteredCollection
    {
      get;
      set;
    } = new AsyncObservableCollection<T>();

    /// <summary>
    /// Constructor
    /// </summary>
    public VsCollectionView() : this(null) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="collection"><see cref="IEnumerable{T}"/></param>
    public VsCollectionView(IEnumerable<T> collection)
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

      _semaphoreLock = new SemaphoreSlim(1, MaxLockCount);
      _semaphoreEstablishQueueLock = new SemaphoreSlim(1, MaxLockCount);
      _collectionQueue = new ConcurrentQueue<T>();
      _internalCollection = new HashSet<T>();

      if ( collection == null )
      {
        Collection = new AsyncObservableCollection<T>();
      }
      else
      {
        IEnumerable<T> enumerable = collection as T[] ?? collection.ToArray();
        Collection = new AsyncObservableCollection<T>(enumerable);
        FilteredCollection = new AsyncObservableCollection<T>(enumerable);
      }

      Collection.CollectionChanged += OnCollectionChanged;
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="List{T}"/>. The value can be null for reference types.</param>
    public void Add(T item) => AddRange(new[] { item });

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="items">he collection whose elements should be added to the end of the <see cref="List{T}"/>.
    /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
    public void AddRange(IEnumerable<T> items) => Collection.AddRange(items);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the <see cref="List{T}"/>.</returns>
    public bool Remove(T item) => Collection.Remove(item);

    /// <summary>
    /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than zero.-or-
    /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.
    /// </exception>
    public void RemoveAt(int index) => Collection.RemoveAt(index);

    /// <summary>
    /// Determines whether an element is in the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="value">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is found in the <see cref="List{T}"/>; otherwise, <c>False</c>.</returns>
    public bool Contains(T value) => FilteredCollection.Contains(value);

    /// <summary>
    /// Clears collection
    /// </summary>
    public void Clear()
    {
      Collection.Clear();
      FilteredCollection.Clear();
      _internalCollection.Clear();
    }

    private async Task EstablishQueueAsync(IEnumerable collection)
    {
      await _semaphoreEstablishQueueLock.WaitAsync(_cts.Token);

      try
      {
        foreach ( object item in collection )
        {
          if ( !(item is T i) )
            continue;

          _collectionQueue.Enqueue(i);
        }

        await RefreshInternalAsync();
      }
      finally
      {
        _semaphoreEstablishQueueLock?.Release();
      }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _semaphoreLock.Wait(_cts.Token);

      try
      {
        switch ( e.Action )
        {
        case NotifyCollectionChangedAction.Add:

          NotifyTaskCompletion.Create(EstablishQueueAsync(e.NewItems));
          break;

        case NotifyCollectionChangedAction.Remove:

          foreach ( object item in e.OldItems )
          {
            if ( item is T i )
            {
              FilteredCollection.Remove(i);
            }
          }

          break;
        }
      }
      finally
      {
        _semaphoreLock.Release();
      }
    }

    private async Task RefreshInternalAsync()
    {
      var sw = new Stopwatch();
      sw.Start();

      try
      {
        if ( _collectionQueue != null )
        {
          var count = 0;
          var page = 0;
          _internalCollection.Clear();

          while ( _collectionQueue.Count > 0 )
          {
            if ( !_collectionQueue.TryDequeue(out var item) )
              continue;

            if ( Filter == null )
            {
              FilteredCollection.Add(item);
            }
            else
            {
              await Task.Run(() =>
              {
                count++;

                if ( Filter(item).Result )
                {
                  _internalCollection.Add(item);
                }
              }, _cts.Token).ContinueWith(p =>
              {
                if ( count < PagingSize )
                  return;

                AddToFilteredCollection(count, page).ConfigureAwait(true);
                count = 0;
                page++;
              }, _cts.Token);
            }
          }

          if ( Filter != null )
          {
            await AddToFilteredCollection(count, page);

            _isFilteringStarted = false;
            FilteringCompleted?.Invoke(this, new FilterEventArgs(true, sw.ElapsedMilliseconds));
          }
        }
      }
      catch ( Exception ex )
      {
        _isFilteringStarted = false;
        FilteringErrorOccurred?.Invoke(this, new FilterEventArgs(false, sw.ElapsedMilliseconds, ex));
      }
    }

    private async Task AddToFilteredCollection(int currentCount, int page)
    {
      await Application.Current.Dispatcher.InvokeAsync(() =>
      {
        var start = page * PagingSize;

        if ( _isFilteringStarted )
        {
          for ( var i = start; i < start + currentCount; i++ )
          {
            var item = Collection[i];

            if ( _internalCollection.Contains(item) )
            {
              FilteredCollection.Add(item);
            }
          }
        }
        else
        {
          FilteredCollection.AddRange(_internalCollection);
        }
      });
    }

    /// <summary>
    /// Release all resources used by <see cref="VsCollectionView{T}"/>
    /// </summary>
    public void Dispose()
    {
      _cts?.Cancel();
      Collection.CollectionChanged -= OnCollectionChanged;

      Clear();

      Collection = null;
      FilteredCollection = null;
      _internalCollection = null;
      Filter = null;
      _collectionQueue = null;

      _semaphoreLock.Dispose();
      _semaphoreEstablishQueueLock.Dispose();
      GC.Collect();
    }
  }
}
