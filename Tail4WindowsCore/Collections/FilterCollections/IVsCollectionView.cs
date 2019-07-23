using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Collections.FilterCollections
{
  /// <summary>
  /// Virtual Studios Collection view interface
  /// </summary>
  /// <typeparam name="T">Type of Collection view</typeparam>
  public interface IVsCollectionView<T> : IDisposable
  {
    /// <summary>
    /// Fires, when the filtering is completed or an <see cref="Exception"/> occurred
    /// </summary>
    event EventHandler<FilterEventArgs> FilteringCompleted;

    /// <summary>
    /// Current filtered items
    /// </summary>
    ObservableCollection<T> Items
    {
      get;
    }

    /// <summary>
    /// Current items count
    /// </summary>
    int Count
    {
      get;
    }

    /// <summary>
    /// Collection view lock
    /// </summary>
    SemaphoreSlim CollectionViewLock
    {
      get;
    }

    /// <summary>
    /// Async filter function
    /// </summary>
    Func<object, Task<bool>> Filter
    {
      get;
      set;
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="List{T}"/>. The value can be null for reference types.</param>
    void Add(T item);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="items">he collection whose elements should be added to the end of the <see cref="List{T}"/>.
    /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
    void AddRange(IEnumerable<T> items);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="List{T}"/>. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the <see cref="List{T}"/>.</returns>
    bool Remove(T item);

    /// <summary>
    /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than zero.-or-
    /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.
    /// </exception>
    void RemoveAt(int index);

    /// <summary>
    /// Clears collection
    /// </summary>
    void Clear();
  }
}
