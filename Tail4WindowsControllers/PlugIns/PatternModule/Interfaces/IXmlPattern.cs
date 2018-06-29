using System.Collections.Generic;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Data;


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
    Task<List<PatternData>> ReadDefaultPatternsAsync();
  }
}
