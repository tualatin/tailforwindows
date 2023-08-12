using System.Windows;
using Org.Vs.Tail4Win.Core.Enums;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
{
  /// <summary>
  /// FileSortMapping
  /// </summary>
  public class FileSortMapping
  {
    /// <summary>
    /// FileSort <see cref="EFileSort"/>
    /// </summary>
    public EFileSort FileSort
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description => Application.Current.TryFindResource(FileSort.GetEnumDescription()).ToString();
  }
}
