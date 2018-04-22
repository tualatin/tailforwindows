using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// A thread save observable
  /// </summary>
  /// <typeparam name="T">Type of Observable</typeparam>
  public class SafeObservable<T> : IList<T>, INotifyCollectionChanged
  {
    private readonly IList<T> _collection = new List<T>();
    private readonly Dispatcher _dispatcher;
    private readonly ReaderWriterLock _sync = new ReaderWriterLock();

    /// <summary>
    /// 
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SafeObservable() => _dispatcher = Dispatcher.CurrentDispatcher;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="collection"><see cref="IEnumerable{T}"/> collection. The collection from which the elements are copied.</param>
    public SafeObservable(IEnumerable<T> collection)
    {
      _dispatcher = Dispatcher.CurrentDispatcher;

      IList<T> items = _collection;

      if ( collection == null || items == null )
        return;

      using ( IEnumerator<T> enumerator = collection.GetEnumerator() )
      {
        while ( enumerator.MoveNext() )
        {
          items.Add(enumerator.Current);
        }
      }
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="Collection{T}"/>
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="Collection{T}"/>. The value can be null for reference types.</param>
    public void Add(T item)
    {
      if ( Thread.CurrentThread == _dispatcher.Thread )
      {
        DoAdd(item);
      }
      else
      {
        _dispatcher.BeginInvoke((Action) (() =>
        {
          DoAdd(item);
        }));
      }
    }

    private void DoAdd(T item)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      _collection.Add(item);
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
      _sync.ReleaseWriterLock();
    }

    /// <summary>
    /// Removes all elements from the <see cref="Collection{T}"/>
    /// </summary>
    public void Clear()
    {
      if ( Thread.CurrentThread == _dispatcher.Thread )
        DoClear();
      else
        _dispatcher.BeginInvoke((Action) DoClear);
    }

    private void DoClear()
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      _collection.Clear();
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      _sync.ReleaseWriterLock();
    }

    /// <summary>
    /// Determines whether an element is in the <see cref="Collection{T}"/>
    /// </summary>
    /// <param name="item">he object to locate in the <see cref="Collection{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is found in the <see cref="Collection{T}"/>; otherwise, <c>False</c>.</returns>
    public bool Contains(T item)
    {
      _sync.AcquireReaderLock(Timeout.Infinite);
      var result = _collection.Contains(item);
      _sync.ReleaseReaderLock();

      return result;
    }

    /// <summary>
    /// Copies the entire <see cref="Collection{T}"/> to a compatible one-dimensional Array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the elements copied from <see cref="Collection{T}"/>. The Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      _collection.CopyTo(array, arrayIndex);
      _sync.ReleaseWriterLock();
    }

    /// <summary>
    /// Gets the number of elements actually contained in the <see cref="Collection{T}"/>.
    /// </summary>
    public int Count
    {
      get
      {
        _sync.AcquireReaderLock(Timeout.Infinite);
        var result = _collection.Count;
        _sync.ReleaseReaderLock();

        return result;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Collection{T}"/> is read-only.
    /// </summary>
    public bool IsReadOnly => _collection.IsReadOnly;

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="Collection{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="Collection{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the original <see cref="Collection{T}"/>.</returns>
    public bool Remove(T item)
    {
      if ( Thread.CurrentThread == _dispatcher.Thread )
        return DoRemove(item);

      var op = _dispatcher.BeginInvoke(new Func<T, bool>(DoRemove), item);

      if ( op.Result == null )
        return false;

      return (bool) op.Result;
    }

    private bool DoRemove(T item)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      var index = _collection.IndexOf(item);
      if ( index == -1 )
      {
        _sync.ReleaseWriterLock();
        return false;
      }
      var result = _collection.Remove(item);
      if ( result && CollectionChanged != null )
        CollectionChanged(this, new
            NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      _sync.ReleaseWriterLock();
      return result;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="Collection{T}"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="Collection{T}"/>.</returns>
    public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _collection.GetEnumerator();

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="Collection{T}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns>The zero-based index of the first occurrence of item within the entire <see cref="Collection{T}"/>, if found; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
      _sync.AcquireReaderLock(Timeout.Infinite);
      var result = _collection.IndexOf(item);
      _sync.ReleaseReaderLock();

      return result;
    }

    /// <summary>
    /// Inserts an element into the <see cref="Collection{T}"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert. The value can be null for reference types.</param>
    public void Insert(int index, T item)
    {
      if ( Thread.CurrentThread == _dispatcher.Thread )
        DoInsert(index, item);
      else
        _dispatcher.BeginInvoke((Action) (() =>
        {
          DoInsert(index, item);
        }));
    }

    private void DoInsert(int index, T item)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      _collection.Insert(index, item);
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
      _sync.ReleaseWriterLock();
    }

    /// <summary>
    /// Removes the element at the specified index of the <see cref="Collection{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public void RemoveAt(int index)
    {
      if ( Thread.CurrentThread == _dispatcher.Thread )
        DoRemoveAt(index);
      else
        _dispatcher.BeginInvoke((Action) (() =>
        {
          DoRemoveAt(index);
        }));
    }

    private void DoRemoveAt(int index)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      if ( _collection.Count == 0 || _collection.Count <= index )
      {
        _sync.ReleaseWriterLock();
        return;
      }
      _collection.RemoveAt(index);
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      _sync.ReleaseWriterLock();
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns></returns>
    public T this[int index]
    {
      get
      {
        _sync.AcquireReaderLock(Timeout.Infinite);
        var result = _collection[index];
        _sync.ReleaseReaderLock();

        return result;
      }
      set
      {
        _sync.AcquireWriterLock(Timeout.Infinite);

        if ( _collection.Count == 0 || _collection.Count <= index )
        {
          _sync.ReleaseWriterLock();
          return;
        }

        _collection[index] = value;
        _sync.ReleaseWriterLock();
      }
    }
  }
}
