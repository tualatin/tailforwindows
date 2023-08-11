using System.Windows;
using Org.Vs.Tail4Win.Core.Enums;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
{
  /// <summary>
  /// Tail refresh rate mapping
  /// </summary>
  public class RefreshRateMapping
  {
    /// <summary>
    /// Tail refresh rate <see cref="ETailRefreshRate"/>
    /// </summary>
    public ETailRefreshRate RefreshRate
    {
      get;
      set;
    }

    /// <summary>
    /// Descritpion
    /// </summary>
    public string Description => Application.Current.TryFindResource(RefreshRate.GetEnumDescription()).ToString();
  }
}
