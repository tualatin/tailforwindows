using System.Threading.Tasks;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Data;
using Org.Vs.TailForWin.Core.Collections;


namespace Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Interfaces
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
