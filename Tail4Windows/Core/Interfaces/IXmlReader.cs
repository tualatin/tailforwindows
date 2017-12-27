using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// XML reader interface
  /// </summary>
  public interface IXmlReader
  {
    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>List of tail settings from XML file</returns>
    Task<ObservableCollection<TailData>> ReadXmlFile();

    /// <summary>
    /// Get list of categories from XML file
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <returns>List of all categories</returns>
    Task<ObservableCollection<string>> GetCategoriesFromXmlFile(ObservableCollection<TailData> tailData);

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <returns>Task</returns>
    Task WriteXmlFile();

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <param name="tailData">TailData to update</param>
    /// <returns>Task</returns>
    Task UpdateTailDataInXmlFile(TailData tailData);

    /// <summary>
    /// Add new tailData to XML file
    /// </summary>
    /// <param name="tailData">TailData to add</param>
    /// <returns>Task</returns>
    Task AddTailDataToXmlFile(TailData tailData);

    /// <summary>
    /// Delete <c>TailData</c> from XML file
    /// </summary>
    /// <param name="id">Id to remove from XML scheme</param>
    /// <returns>Task</returns>
    Task DeleteTailDataByIdFromXmlFile(string id);

    /// <summary>
    /// Delete a filter element from XML file
    /// </summary>
    /// <param name="id">Id of parent XML element</param>
    /// <param name="filterId">Id of filter to remove</param>
    /// <returns>Task</returns>
    Task DeleteFilterByIdByTailDataIdFromXmlFile(string id, string filterId);

    /// <summary>
    /// Get <c>TailData</c> by certain Id
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <param name="id">Id</param>
    /// <returns><c>TailData</c>, otherwise <c>Null</c></returns>
    Task<TailData> GetTailDataById(ObservableCollection<TailData> tailData, Guid id);
  }
}
