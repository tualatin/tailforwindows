using System.Windows;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;

namespace Org.Vs.TailForWin.Core.Data.Mappings
{
  /// <summary>
  /// SmartWatch mapping
  /// </summary>
  public class SmartWatchMapping
  {
    /// <summary>
    /// <see cref="ESmartWatchMode"/>
    /// </summary>
    public ESmartWatchMode SmartWatchMode
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description => Application.Current.TryFindResource(SmartWatchMode.GetEnumDescription()).ToString();
  }
}
