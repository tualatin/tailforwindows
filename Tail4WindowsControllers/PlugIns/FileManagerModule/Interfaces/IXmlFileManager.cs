using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// XML FileManager interface
  /// </summary>
  [Obsolete("Please use IFileManager instead")]
  public interface IXmlFileManager
  {
    /// <summary>
    /// Gets current XML config file
    /// </summary>
    string XmlFileName
    {
      get;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>List of tail settings from XML file</returns>
    Task<ObservableCollection<TailData>> ReadXmlFileAsync(CancellationToken token);

    /// <summary>
    /// Get list of categories from XML file
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <returns>List of all categories</returns>
    Task<ObservableCollection<string>> GetCategoriesFromXmlFileAsync(ObservableCollection<TailData> tailData);

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    Task WriteXmlFileAsync(CancellationToken token);

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">TailData to update</param>
    /// <returns>Task</returns>
    Task UpdateTailDataInXmlFileAsync(CancellationToken token, TailData tailData);

    /// <summary>
    /// Add new tailData to XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">TailData to add</param>
    /// <returns>Task</returns>
    Task AddTailDataToXmlFileAsync(CancellationToken token, TailData tailData);

    /// <summary>
    /// Delete <c>TailData</c> from XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="id">Id to remove from XML scheme</param>
    /// <returns>Task</returns>
    Task DeleteTailDataByIdFromXmlFileAsync(CancellationToken token, string id);

    /// <summary>
    /// Delete a filter element from XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="id">Id of parent XML element</param>
    /// <param name="filterId">Id of filter to remove</param>
    /// <returns>Task</returns>
    Task DeleteFilterByIdByTailDataIdFromXmlFileAsync(CancellationToken token, string id, string filterId);

    /// <summary>
    /// Get <c>TailData</c> by certain Id
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <param name="id">Id</param>
    /// <returns><c>TailData</c>, otherwise <c>Null</c></returns>
    Task<TailData> GetTailDataByIdAsync(ObservableCollection<TailData> tailData, Guid id);
  }
}
