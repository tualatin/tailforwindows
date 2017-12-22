using System.Collections.Generic;
using System.Linq;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Extensions
{
  /// <summary>
  /// Enumerable extension
  /// </summary>
  public static class EnumerableExtension
  {
    /// <summary>
    /// Has duplicates
    /// </summary>
    /// <typeparam name="T">Type of enumerable</typeparam>
    /// <param name="subjects">Enumerable</param>
    /// <returns><c>True</c> if it has dublicates, otherwise <c>False</c></returns>
    public static bool HasDuplicates<T>(this IEnumerable<T> subjects)
    {
      return HasDuplicates(subjects, EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Has duplicates
    /// </summary>
    /// <typeparam name="T">Type of enumerable</typeparam>
    /// <param name="subjects">Enumerable</param>
    /// <param name="comparer">Comparer</param>
    /// <returns><c>True</c> if it has dublicates, otherwise <c>False</c></returns>
    public static bool HasDuplicates<T>(this IEnumerable<T> subjects, IEqualityComparer<T> comparer)
    {
      Arg.NotNull(subjects, nameof(subjects));
      Arg.NotNull(comparer, nameof(comparer));

      var set = new HashSet<T>(comparer);

      return subjects.Any(s => !set.Add(s));
    }

    /// <summary>
    /// Find a dublicate in an Observable collection
    /// </summary>
    /// <typeparam name="T">Type of Observable collection</typeparam>
    /// <param name="subjects">Enumerable</param>
    /// <param name="newValue">Value to add</param>
    /// <param name="comparer">Comparer</param>
    /// <returns><c>True</c> if dublicate found, otherwise <c>False</c></returns>
    public static bool FindDublicates<T>(this IEnumerable<T> subjects, T newValue, IEqualityComparer<T> comparer)
    {
      Arg.NotNull(newValue, nameof(newValue));
      Arg.NotNull(subjects, nameof(subjects));

      if ( comparer == null )
        comparer = EqualityComparer<T>.Default;

      var set = new HashSet<T>(comparer);

      foreach ( var subject in subjects )
      {
        set.Add(subject);
      }
      return !set.Add(newValue);
    }
  }
}
