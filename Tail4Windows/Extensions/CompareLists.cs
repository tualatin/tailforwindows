using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace Org.Vs.TailForWin.Extensions
{
  /// <summary>
  /// Compare generic lists
  /// </summary>
  public static class CompareLists
  {
    /// <summary>
    /// Compare generic lists
    /// </summary>
    /// <typeparam name="T">Type of list</typeparam>
    /// <param name="firstList">First list</param>
    /// <param name="secondList">Second list</param>
    /// <returns>If lists equal <c>True</c> otherwise <c>False</c></returns>
    internal static bool CompareGenericLists<T>(List<T> firstList, List<T> secondList)
    {
      return (firstList.Count == secondList.Count // assumes unique values in each list
          && new HashSet<T>(firstList).SetEquals(secondList));
    }

    internal static bool CompareGenericObservableCollections<T>(ObservableCollection<T> first, ObservableCollection<T> second)
    {
      IEnumerable<T> result = first.Except(second);

      if(result.Count() > 0)
        return (false);

      return (true);
    }
  }
}
