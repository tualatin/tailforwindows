using System.Collections.Generic;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// Default pattern structure controller interface
  /// </summary>
  public interface IDefaultPatternStructureController
  {
    /// <summary>
    /// Reads the default pattern XML file
    /// </summary>
    /// <returns>List of default patterns</returns>
    List<Pattern> ReadDefaultPatternFile();
  }
}