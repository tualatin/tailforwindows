using Org.Vs.Tail4Win.Core.Enums;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
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
