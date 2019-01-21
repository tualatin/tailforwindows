using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// FileManager controller interface
  /// </summary>
  public interface IFileManagerController
  {
    /// <summary>
    /// Converts old XML config file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ConvertXmlToJsonConfigAsync(CancellationToken token);

    /// <summary>
    /// Reads a JSON file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>List of tail settings from JSON file</returns>
    Task<ObservableCollection<TailData>> ReadJsonFileAsync(CancellationToken token);
  }
}
