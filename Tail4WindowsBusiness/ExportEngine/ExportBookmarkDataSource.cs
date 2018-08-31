using System.Collections.Generic;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.ExportEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Business.ExportEngine
{
  /// <summary>
  /// Export bookmark data source
  /// </summary>
  public class ExportBookmarkDataSource : IDataExport<LogEntry>
  {
    /// <summary>
    /// Export data as CSV
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ExportAsCsvAsync(IList<LogEntry> data, string fileName)
    {
      bool result = false;

      await Task.Run(() =>
      {

      });
      return result;
    }

    /// <summary>
    /// Export data as Excel sheet
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ExportAsExcelAsync(IList<LogEntry> data, string fileName) => throw new System.NotImplementedException();

    /// <summary>
    /// Export data as OpenDocument
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ExportAsOpenDocumentAsync(IList<LogEntry> data, string fileName) => throw new System.NotImplementedException();
  }
}
