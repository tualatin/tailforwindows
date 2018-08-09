using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
// ReSharper disable PossibleMultipleEnumeration


namespace Org.Vs.TailForWin.Core.Extensions
{
  /// <summary>
  /// <see cref="ObservableCollection{T}"/> extension
  /// </summary>
  public static class ObservableCollectionExtension
  {
    /// <summary>
    /// Missing AddRange extension for <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of <see cref="ObservableCollection{T}"/></typeparam>
    /// <param name="collection">Collection</param>
    /// <param name="range">Range to add</param>
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
    {
      if ( range == null || !range.Any() )
        return;

      foreach ( T item in range )
      {
        collection.Add(item);
      }
    }
  }
}
