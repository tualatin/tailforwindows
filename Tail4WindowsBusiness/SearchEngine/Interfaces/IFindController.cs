using System.Collections.Generic;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.SearchEngine.Interfaces
{
  /// <summary>
  /// FindController interface
  /// </summary>
  public interface IFindController
  {
    /// <summary>
    /// FindControll is busy indicator
    /// </summary>
    bool IsBusy
    {
      get;
    }

    /// <summary>
    /// Mathes a text
    /// </summary>
    /// <param name="findSettings">Current find settings <see cref="FindData"/></param>
    /// <param name="value">Value as string</param>
    /// <param name="pattern">Search pattern</param>
    /// <returns>List of valid strings, otherwise null</returns>
    Task<List<string>> MatchTextAsync(FindData findSettings, string value, string pattern);
  }
}
