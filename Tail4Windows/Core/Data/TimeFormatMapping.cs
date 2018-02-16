using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Core.Data
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
