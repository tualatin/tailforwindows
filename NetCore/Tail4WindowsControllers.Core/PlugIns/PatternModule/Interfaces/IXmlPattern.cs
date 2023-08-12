using Org.Vs.Tail4Win.Controllers.PlugIns.PatternModule.Data;
using Org.Vs.Tail4Win.Core.Collections;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.PatternModule.Interfaces
{
  /// <summary>
  /// XML pattern interface
  /// </summary>
  public interface IXmlPattern
  {
    /// <summary>
    /// Reads default patterns for SmartWatch
    /// </summary>
    /// <returns>List of default patterns</returns>
    Task<AsyncObservableCollection<PatternData>> ReadDefaultPatternsAsync();
  }
}
