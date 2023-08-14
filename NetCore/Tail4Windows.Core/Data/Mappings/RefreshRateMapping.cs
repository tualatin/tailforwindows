using System.Windows;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;

namespace Org.Vs.TailForWin.Core.Data.Mappings
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
