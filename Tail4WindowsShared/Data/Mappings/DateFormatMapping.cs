using Org.Vs.Tail4Win.Shared.Enums;
using Org.Vs.Tail4Win.Shared.Extensions;

namespace Org.Vs.Tail4Win.Shared.Data.Mappings
{
  /// <summary>
  /// DateFormatMapping
  /// </summary>
  public class DateFormatMapping
  {
    /// <summary>
    /// Date format as <see cref="EDateFormat"/>
    /// </summary>
    public EDateFormat DateFormat
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description => DateFormat.GetEnumDescription();
  }
}
