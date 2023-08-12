using Org.Vs.Tail4Win.Core.Data.Base;

namespace Org.Vs.Tail4Win.Business.ExportEngine.Interfaces
{
  /// <summary>
  /// Data export interface
  /// </summary>
  /// <typeparam name="T">T must a type of <see cref="NotifyMaster"/></typeparam>
  public interface IDataExport<T> where T : NotifyMaster
  {
    /// <summary>
    /// Export data as CSV
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ExportAsCsvAsync(IList<T> data, string fileName);

    /// <summary>
    /// Export data as Excel sheet
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ExportAsExcelAsync(IList<T> data, string fileName);

    /// <summary>
    /// Export data as OpenDocument
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ExportAsOpenDocumentAsync(IList<T> data, string fileName);
  }
}
