using Org.Vs.Tail4Win.Shared.Enums;
using Org.Vs.Tail4Win.Shared.Extensions;

namespace Org.Vs.Tail4Win.Shared.Data.Mappings
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
