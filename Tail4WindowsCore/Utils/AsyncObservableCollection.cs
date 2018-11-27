using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Async <see cref="ObservableCollection{T}"/>
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [Serializable]
  [CollectionDataContract]
  [ComVisible(false)]
  [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
  public class AsyncObservableCollection<T> : IList<T>, IList, IReadOnlyList<T>, INotifyPropertyChanged
  {
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
    private readonly object _myLock = new object();

    /// <summary>
    /// Hash collection
    /// </summary>
    private HashSet<T> _hashCollection;

    /// <summary>
    /// Synchronisation
    /// </summary>
    [NonSerialized]
    private ReaderWriterLockSlim _sync;

    /// <summary>
    /// Queue with <see cref="NotifyCollectionChangedEventArgs"/>
    /// </summary>
    private ConcurrentQueue<NotifyCollectionChangedEventArgs> _collectionChangedQueue;

    /// <summary>
    /// Source of collection
    /// </summary>
    public ObservableCollection<T> Items
    {
      get;
      private set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public AsyncObservableCollection() => Initialize();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="list">Initialize <see cref="IEnumerable{T}"/></param>
    public AsyncObservableCollection(IEnumerable<T> list)
    {
      Initialize();
      AddRange(list);
    }

    private void Initialize()
    {
      Items = new ObservableCollection<T>();
      Items.CollectionChanged += CollectionCollectionChanged;

      _sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      _collectionChangedQueue = new ConcurrentQueue<NotifyCollectionChangedEventArgs>();
      _hashCollection = new HashSet<T>();

      BindingOperations.EnableCollectionSynchronization(Items, _myLock);
    }

    #region IEnumerable<T> Members

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.List`1" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.List`1.Enumerator" /> for the <see cref="T:System.Collections.Generic.List`1" />.
    /// </returns>
    public IEnumerator<T> GetEnumerator()
    {
      try
      {
        _sync.EnterReadLock();
        return Items.ToList().GetEnumerator();
      }
      finally
      {
        _sync.ExitReadLock();
      }
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.List`1" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.List`1.Enumerator" /> for the <see cref="T:System.Collections.Generic.List`1" />.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region INotifyCollectionChanging Members

    /// <summary>
    /// Collection changing event
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanging;

    #endregion

    private void CollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if ( SynchronizationContext.Current == _synchronizationContext )
      {
        // Execute the CollectionChanged event on the current thread
        RaiseCollectionChanged(e);
      }
      else
      {
        // Raises the CollectionChanged event on the creator thread
        _synchronizationContext.Send(RaiseCollectionChanged, e);
      }
    }

    private void RaiseCollectionChanged(object param)
    {
      if ( !(param is NotifyCollectionChangedEventArgs e) )
        return;

      _collectionChangedQueue.Enqueue(e);
      ProcessQueue();
    }

    #region Move

    /// <summary>
    /// Moves the item at the specified index to a new location in the collection.
    /// </summary>
    /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
    /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
    public void Move(int oldIndex, int newIndex)
    {
      try
      {
        _sync.EnterWriteLock();
        Items.Move(oldIndex, newIndex);
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    #endregion

    #region ToList

    /// <summary>
    /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> that contains elements from the input sequence.</returns>
    public List<T> ToList()
    {
      List<T> newList;

      try
      {
        _sync.EnterWriteLock();
        newList = new List<T>(Items);
      }
      finally
      {
        _sync.ExitWriteLock();
      }
      return newList;
    }

    #endregion

    private int _accessCount;

    private void ProcessQueue()
    {
      if ( Monitor.TryEnter(_myLock) )
      {
        try
        {
          if ( _accessCount > 0 )
            return;

          _accessCount++;
        }
        finally
        {
          Monitor.Exit(_myLock);
        }
      }

      if ( _sync.IsReadLockHeld || _sync.IsUpgradeableReadLockHeld || _sync.IsWriteLockHeld )
      {
        if ( Monitor.TryEnter(_myLock) )
        {
          try
          {
            _accessCount--;
          }
          finally
          {
            Monitor.Exit(_myLock);
          }
        }
        return;
      }

      if ( _collectionChangedQueue == null )
      {
        _collectionChangedQueue = new ConcurrentQueue<NotifyCollectionChangedEventArgs>();

        if ( Monitor.TryEnter(_myLock) )
        {
          try
          {
            _accessCount--;
          }
          finally
          {
            Monitor.Exit(_myLock);
          }
        }
        return;
      }

      while ( _collectionChangedQueue.Count > 0 )
      {
        if ( _collectionChangedQueue.TryDequeue(out NotifyCollectionChangedEventArgs args) )
          OnCollectionChanged(args);
      }

      if ( Monitor.TryEnter(_myLock) )
      {
        try
        {
          _accessCount--;
        }
        finally
        {
          Monitor.Exit(_myLock);
        }
      }
    }

    #region IList<T> Members

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="item" /> within the entire <see cref="T:System.Collections.ObjectModel.Collection`1" />, if found; otherwise, -1.
    /// </returns>
    public int IndexOf(T item)
    {
      try
      {
        _sync.EnterReadLock();
        return Items.IndexOf(item);
      }
      finally
      {
        _sync.ExitReadLock();
      }
    }

    /// <summary>
    /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert. The value can be <see langword="null" /> for reference types.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than zero.-or-
    /// <paramref name="index" /> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.</exception>
    public void Insert(int index, T item)
    {
      try
      {
        _sync.EnterWriteLock();
        var oldIndex = Items.IndexOf(item);

        if ( oldIndex != -1 )
        {
          if ( oldIndex < Items.Count )
          {
            Items.Move(index, oldIndex);
          }
        }
        else
        {
          Items.Insert(index, item);
          _hashCollection.Add(item);
        }
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    /// <summary>
    /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than zero.-or-
    /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.
    /// </exception>
    public void RemoveAt(int index)
    {
      try
      {
        _sync.EnterWriteLock();
        var item = Items[index];

        _hashCollection.Remove(item);
        Items.RemoveAt(index);
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    /// <summary>
    /// Gets / sets object by index
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>object</returns>
    /// <exception cref="InvalidOperationException"> if index is not in range</exception>
    public T this[int index]
    {
      get
      {
        try
        {
          _sync.EnterReadLock();
          return Items[index];
        }
        finally
        {
          _sync.ExitReadLock();
        }
      }
      set
      {
        try
        {
          _sync.EnterWriteLock();

          if ( _hashCollection.Contains(value) )
          {
            var oldIndex = Items.IndexOf(value);
            Items.Move(oldIndex, index);
          }
          else
          {
            var oldItem = Items[index];

            _hashCollection.Remove(oldItem);
            _hashCollection.Add(value);
            Items[index] = value;
          }
        }
        finally
        {
          _sync.ExitWriteLock();
          ProcessQueue();
        }
      }
    }

    #endregion

    #region ICollection<T> Members

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
    public void AddRange(IEnumerable<T> items) => AddRangeInternal(items);

    private void AddRangeInternal(IEnumerable<T> items)
    {
      if ( items == null )
        return;

      try
      {
        _sync.EnterUpgradeableReadLock();

        foreach ( var item in items )
        {
          if ( _hashCollection.Contains(item) )
            continue;

          try
          {
            _sync.EnterWriteLock();
            _hashCollection.Add(item);
            Items.Add(item);
          }
          finally
          {
            _sync.ExitWriteLock();
          }
        }
      }
      finally
      {
        _sync.ExitUpgradeableReadLock();
        ProcessQueue();
      }
    }

    /// <summary>
    /// Removes all elements from the <see cref="List{T}"/>.
    /// </summary>
    public void Clear()
    {
      try
      {
        _sync.EnterWriteLock();
        Items.ToList().ForEach(p => Items.Remove(p));
        _hashCollection.Clear();
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    /// <summary>
    /// Determines whether an element is in the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is found in the <see cref="List{T}"/>; otherwise, <c>False</c>.</returns>
    public bool Contains(T item)
    {
      try
      {
        _sync.EnterReadLock();
        return _hashCollection.Contains(item);
      }
      finally
      {
        _sync.ExitReadLock();
      }
    }

    /// <summary>
    /// Copies the <see cref="List{T}"/> or a portion of it to an array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="List{T}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <see cref="Array"/> at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      try
      {
        _sync.EnterWriteLock();
        if ( Items.Count - arrayIndex > array.Length )
        {
          return;
        }
        Items.CopyTo(array, arrayIndex);
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="List{T}"/>.
    /// </summary>
    public int Count
    {
      get
      {
        try
        {
          _sync.EnterReadLock();
          return Items.Count;
        }
        finally
        {
          _sync.ExitReadLock();
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
    /// </summary>
    public bool IsReadOnly => ((IList) Items).IsReadOnly;

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the <see cref="List{T}"/>.</returns>
    public bool Remove(T item)
    {
      try
      {
        _sync.EnterWriteLock();
        _hashCollection.Remove(item);

        return Items.Remove(item);
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    #endregion

    #region IList Members

    /// <summary>
    /// Adds an object to the end of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="value">The object to be added to the end of the <see cref="List{T}"/>. The value can be null for reference types.</param>
    int IList.Add(object value)
    {
      Add((T) value);
      return IndexOf((T) value);
    }

    /// <summary>
    /// Removes all elements from the <see cref="List{T}"/>.
    /// </summary>
    void IList.Clear() => Clear();

    /// <summary>
    /// Determines whether an element is in the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="value">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is found in the <see cref="List{T}"/>; otherwise, <c>False</c>.</returns>
    bool IList.Contains(object value) => Contains((T) value);

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="value">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="value" /> within the entire <see cref="T:System.Collections.ObjectModel.Collection`1" />, if found; otherwise, -1.
    /// </returns>
    int IList.IndexOf(object value) => IndexOf((T) value);

    /// <summary>
    /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
    /// <param name="value">The object to insert. The value can be <see langword="null" /> for reference types.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than zero.-or-
    /// <paramref name="index" /> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.</exception>
    void IList.Insert(int index, object value) => Insert(index, (T) value);

    /// <summary>
    /// Gets a value indicating whether the <see cref="IList"/> has a fixed size.
    /// </summary>
    bool IList.IsFixedSize => IsFixedSize;

    /// <summary>
    /// Gets a value indicating whether the <see cref="IList"/> has a fixed size.
    /// </summary>
    protected bool IsFixedSize => ((IList) Items).IsFixedSize;

    /// <summary>
    /// Gets a value indicating whether the <see cref="IList"/> is read-only.
    /// </summary>
    bool IList.IsReadOnly => ((IList) Items).IsReadOnly;

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="value">The object to remove from the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the <see cref="List{T}"/>.</returns>
    void IList.Remove(object value) => Remove((T) value);

    /// <summary>
    /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than zero.-or-
    /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.
    /// </exception>
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// Gets / sets object by index
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>object</returns>
    /// <exception cref="InvalidOperationException"> if index is not in range</exception>
    object IList.this[int index]
    {
      get => this[index];
      set => this[index] = (T) value;
    }

    #endregion

    #region ICollection Members

    /// <summary>
    /// Copies the <see cref="List{T}"/> or a portion of it to an array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="List{T}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in <see cref="Array"/> at which copying begins.</param>
    void ICollection.CopyTo(Array array, int index)
    {
      try
      {
        _sync.EnterWriteLock();

        if ( Items.Count - index > array.Length )
          return;

        ((ICollection) Items).CopyTo(array, index);
      }
      finally
      {
        _sync.ExitWriteLock();
        ProcessQueue();
      }
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="List{T}"/>.
    /// </summary>
    int ICollection.Count => Count;

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
    /// </summary>
    bool ICollection.IsSynchronized => IsSynchronized;

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
    /// </summary>
    protected bool IsSynchronized => ((ICollection) Items).IsSynchronized;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="ICollection"/>.
    /// </summary>
    object ICollection.SyncRoot => SyncRoot;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="ICollection"/>.
    /// </summary>
    protected object SyncRoot => ((ICollection) Items).SyncRoot;

    #endregion

    #region INotifyCollectionChanged Members

    /// <summary>
    /// Represents the method that handles the CollectionChanged event raised when adding elements to or removing elements from a collection.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Fires, when the collection changed
    /// </summary>
    /// <param name="ea"><see cref="NotifyCollectionChangedEventArgs"/></param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs ea)
    {
      RaiseCollectionChanging(ea);
      CollectionChanged?.Invoke(this, ea);

      if ( ea.Action == NotifyCollectionChangedAction.Add ||
           ea.Action == NotifyCollectionChangedAction.Remove ||
           ea.Action == NotifyCollectionChangedAction.Reset )
      {
        OnPropertyChanged(nameof(Count));
      }
    }

    private void RaiseCollectionChanging(NotifyCollectionChangedEventArgs args)
    {
      var changing = CollectionChanging;
      changing?.Invoke(this, args);
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    protected virtual void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
  }
}
