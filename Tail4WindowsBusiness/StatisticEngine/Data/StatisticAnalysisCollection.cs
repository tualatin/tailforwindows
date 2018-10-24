using System;
using System.Collections;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.StatisticEngine.Data
{
  /// <summary>
  /// Statistic analysis collection
  /// </summary>
  public class StatisticAnalysisCollection : IStatisticAnalysisCollection
  {
    private readonly AsyncObservableCollection<StatisticAnalysisData> _statisticCollection;
    private int _currentIndex;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StatisticAnalysisCollection()
    {
      _currentIndex = -1;
      _statisticCollection = new AsyncObservableCollection<StatisticAnalysisData>();
    }

    /// <summary>
    /// Adds an object to the end of the List.
    /// </summary>
    /// <param name="item">The object to be added to the end of the List. The value can be null for reference types.</param>
    public void Add(StatisticAnalysisData item)
    {
      if ( item != null )
        _statisticCollection.Add(item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the List.
    /// </summary>
    /// <param name="item">The object to remove from the List. The value can be null for reference types.</param>
    /// <returns><c>True</c> if item is successfully removed; otherwise, <c>False</c>. This method also returns false if item was not found in the List.</returns>
    public bool Remove(StatisticAnalysisData item) => item != null && _statisticCollection.Remove(item);

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a value in the List or in a portion of it.
    /// </summary>
    /// <param name="item">The object to locate in the List. The value can be null for reference types.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of item within the range of elements in the List that extends from index to the last element, if found; otherwise, –1.
    /// </returns>
    public int IndexOf(StatisticAnalysisData item) => item != null ? _statisticCollection.IndexOf(item) : -1;

    /// <summary>
    /// Removes all elements from the List.
    /// </summary>
    public void Clear() => _statisticCollection.Clear();

    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns><c>True</c> if the enumerator was successfully advanced to the next element; <c>False</c> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext()
    {
      _currentIndex++;
      return _currentIndex < _statisticCollection.Count;
    }

    /// <summary>
    /// Sets the enumerator to its initial position, which is before the first element in the collection.
    /// </summary>
    public void Reset() => _currentIndex = -1;

    /// <summary>
    /// Gets the element in the collection at the current position of the enumerator.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if current index is out of range</exception>
    public object Current
    {
      get
      {
        try
        {
          return _statisticCollection[_currentIndex];
        }
        catch ( IndexOutOfRangeException )
        {
          throw new InvalidOperationException();
        }
      }
    }

    /// <summary>
    /// Gets <see cref="StatisticAnalysisData"/> by index
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns><see cref="StatisticAnalysisData"/></returns>
    /// <exception cref="InvalidOperationException"> if index is not in range</exception>
    public StatisticAnalysisData this[int index]
    {
      get
      {
        try
        {
          return _statisticCollection[index];
        }
        catch ( IndexOutOfRangeException )
        {
          throw new InvalidOperationException();
        }
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    public IEnumerator GetEnumerator() => _statisticCollection.GetEnumerator();
  }
}
