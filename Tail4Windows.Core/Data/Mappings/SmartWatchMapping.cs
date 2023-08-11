using System.Windows;
using Org.Vs.Tail4Win.Core.Enums;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
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
