using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Core.Data.Mappings
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
