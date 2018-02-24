using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Core.Data.Mappings
{
  /// <summary>
  /// WindowStyleMapping
  /// </summary>
  public class WindowStyleMapping
  {
    /// <summary>
    /// WindowStyle as <see cref="EWindowStyle"/>
    /// </summary>
    public EWindowStyle WindowStyle
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description => WindowStyle.GetEnumDescription();
  }
}
