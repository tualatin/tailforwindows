using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// JSON history interface
  /// </summary>
  public interface IHistory
  {
    /// <summary>
    /// Converts old XML history file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ConvertXmlToJsonFileAsync(CancellationToken token);

    /// <summary>
    /// Reads history file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="HistoryData"/></returns>
    Task<HistoryData> ReadHistoryAsync(CancellationToken token);

    /// <summary>
    /// Saves a search text to history file
    /// </summary>
    /// <param name="searchText">Text to save</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> SaveHistoryAsync(string searchText, CancellationToken token);
  }
}
