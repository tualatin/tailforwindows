using Org.Vs.Tail4Win.Shared.Enums;
using Org.Vs.Tail4Win.Shared.Extensions;

namespace Org.Vs.Tail4Win.Shared.Data.Mappings
{
  /// <summary>
  /// TimeFormatMapping
  /// </summary>
  public class TimeFormatMapping
  {
    /// <summary>
    /// TimeFormat as <see cref="ETimeFormat"/>
    /// </summary>
    public ETimeFormat TimeFormat
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description => TimeFormat.GetEnumDescription();
  }
}
