using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace Org.Vs.TailForWin.Extensions
{
  /// <summary>
  /// Clone a ObservableCollection
  /// </summary>
  public static class CloneObservableCollection
  {
    /// <summary>
    /// Make a deep copy of an ObservableCollection, have to implement interface <c>ICloneable</c>
    /// </summary>
    /// <typeparam name="T">Type of collection</typeparam>
    /// <param name="list">List to copy</param>
    /// <returns>Copy of ObservableCollection</returns>
    internal static ObservableCollection<T> DeepCopy<T>(IEnumerable<T> list) where T : ICloneable
    {
      return (new ObservableCollection<T>(list.Select(x => x.Clone()).Cast<T>()));
    }
  }
}
