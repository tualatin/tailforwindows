using System.Collections.Generic;
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

    /// <summary>
    /// Compare generic IEnumerables
    /// </summary>
    /// <typeparam name="T">Type of IEnumerable</typeparam>
    /// <param name="first">First IEnumerable</param>
    /// <param name="second">Second IEnumerable</param>
    /// <returns>If IEnumerables equal <c>True</c> otherwise <c>False</c></returns>
    internal static bool CompareGenericObservableCollections<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
      IEnumerable<T> result = first.Except(second);

      if(result.Any())
        return (false);

      return (true);
    }
  }
}
