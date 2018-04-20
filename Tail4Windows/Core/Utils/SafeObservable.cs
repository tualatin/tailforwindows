using System;
using System.Collections.Generic;
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
    public event NotifyCollectionChangedEventHandler CollectionChanged;
    private readonly ReaderWriterLock _sync = new ReaderWriterLock();

    public SafeObservable() => _dispatcher = Dispatcher.CurrentDispatcher;

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

    public void Add(T item)
    {
      if ( Thread.CurrentThread == _dispatcher.Thread )
        DoAdd(item);
      else
        _dispatcher.BeginInvoke((Action) (() =>
        {
          DoAdd(item);
        }));
    }

    private void DoAdd(T item)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      _collection.Add(item);
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
      _sync.ReleaseWriterLock();
    }

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

    public bool Contains(T item)
    {
      _sync.AcquireReaderLock(Timeout.Infinite);
      var result = _collection.Contains(item);
      _sync.ReleaseReaderLock();

      return result;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _sync.AcquireWriterLock(Timeout.Infinite);
      _collection.CopyTo(array, arrayIndex);
      _sync.ReleaseWriterLock();
    }

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

    public bool IsReadOnly => _collection.IsReadOnly;

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

    public IEnumerator<T> GetEnumerator()
    {
      return _collection.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _collection.GetEnumerator();
    }

    public int IndexOf(T item)
    {
      _sync.AcquireReaderLock(Timeout.Infinite);
      var result = _collection.IndexOf(item);
      _sync.ReleaseReaderLock();

      return result;
    }

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
