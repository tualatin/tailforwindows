using System.Collections.ObjectModel;

namespace Org.Vs.Tail4Win.Core.Collections
{
  /// <summary>
  /// A generic queue set
  /// </summary>
  /// <typeparam name="T">Type of set</typeparam>
  public class QueueSet<T> : ICollection<T>
  {
    private readonly ObservableCollection<T> _queue = new ObservableCollection<T>();
    private readonly int _maximumSize;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="maximumSize">Max size of queue</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public QueueSet(int maximumSize)
    {
      if ( maximumSize < 0 )
        throw new ArgumentOutOfRangeException(nameof(maximumSize));

      _maximumSize = maximumSize;
    }
  
    /// <summary>
    /// Dequeue first element in queue
    /// </summary>
    /// <returns>Removed element</returns>
    public T Dequeue()
    {
      if ( _queue.Count <= 0 )
        return default(T);

      var value = _queue[0];
      _queue.RemoveAt(0);

      return value;
    }

    /// <summary>
    /// Get numbers of elements in queue
    /// </summary>
    /// <returns>Number of elements</returns>
    public T Peek() => _queue.Count > 0 ? _queue[0] : default(T);

    /// <summary>
    /// Enqueue a element to last position, removes first element in queue, if max size exceeded
    /// </summary>
    /// <param name="item">Item to insert</param>
    public void Enqueue(T item)
    {
      if ( _queue.Contains(item) )
        _queue.Remove(item);

      _queue.Add(item);

      while ( _queue.Count > _maximumSize )
      {
        Dequeue();
      }
    }

    /// <summary>
    /// Length of queue
    /// </summary>
    public int Count => _queue.Count;

    /// <summary>
    /// Is readOnly, always false
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Add a new element to queue
    /// </summary>
    /// <param name="item">Item to add</param>
    public void Add(T item) => Enqueue(item);

    /// <summary>
    /// Clears the complete queue
    /// </summary>
    public void Clear() => _queue.Clear();

    /// <summary>
    /// Contains a certain element
    /// </summary>
    /// <param name="item">Item to search</param>
    /// <returns><c>True</c> if found, otherwise <c>False</c></returns>
    public bool Contains(T item) => _queue.Contains(item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      foreach ( var value in _queue )
      {
        if ( arrayIndex >= array.Length )
          break;

        if ( arrayIndex >= 0 )
          array[arrayIndex] = value;

        arrayIndex++;
      }
    }

    /// <summary>
    /// Removes a item
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns><c>True</c> if successfully removed, otherwise <c>False</c></returns>
    public bool Remove(T item)
    {
      if ( Equals(item, Peek()) )
      {
        Dequeue();
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// GetEnumerator
    /// </summary>
    /// <returns><see cref="IEnumerator{T}"/></returns>
    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _queue.GetEnumerator();
  }
}
