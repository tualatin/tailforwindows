using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Core.Data.Mappings
{
  /// <summary>
  /// FileSortMapping
  /// </summary>
  public class FileSortMapping
  {
    /// <summary>
    /// FileSort <see cref="EFileSort"/>
    /// </summary>
    public EFileSort FileSort
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description => FileSort.GetEnumDescription();
  }
}
