﻿using System.Collections;

namespace Org.Vs.TailForWin.Business.StatisticEngine.Interfaces
{
  /// <summary>
  /// Statistic analysis collection interface
  /// </summary>
  /// <typeparam name="T">Type of interface</typeparam>
  public interface IStatisticAnalysisCollection<T> : IEnumerator, IEnumerable<T>
  {
    /// <summary>
    /// Gets data by index
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>Data</returns>
    /// <exception cref="System.InvalidOperationException"> if index is not in range</exception>
    T this[int index]
    {
      get;
    }

    /// <summary>
    /// Gets the number of elements contained in the List.
    /// </summary>
    int Count
    {
      get;
    }

    /// <summary>
    /// Adds an object to the end of the List.
    /// </summary>
    /// <param name="item">The object to be added to the end of the List. The value can be null for reference types.</param>
    void Add(T item);

    /// <summary>
    /// Removes the first occurrence of a specific object from the List.
    /// </summary>
    /// <param name="item">The object to remove from the List. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the List.</returns>
    bool Remove(T item);

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a value in the List or in a portion of it.
    /// </summary>
    /// <param name="item">The object to locate in the List. The value can be null for reference types.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of item within the range of elements in the List that extends from index to the last element, if found; otherwise, –1.
    /// </returns>
    int IndexOf(T item);

    /// <summary>
    /// Removes all elements from the List.
    /// </summary>
    void Clear();

    /// <summary>
    /// Orders the current collection by <see cref="DbScheme.SessionEntity"/> date
    /// </summary>
    void OrderCollectionByDate();
  }
}
