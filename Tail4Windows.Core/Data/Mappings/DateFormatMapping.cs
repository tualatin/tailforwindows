using Org.Vs.Tail4Win.Core.Enums;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
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
