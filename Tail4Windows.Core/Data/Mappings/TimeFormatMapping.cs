using Org.Vs.Tail4Win.Core.Enums;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
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
