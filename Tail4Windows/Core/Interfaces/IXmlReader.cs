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
    /// Read XML config file
    /// </summary>
    /// <returns>List of tail settings from XML file</returns>
    Task<ObservableCollection<TailData>> ReadXmlFile();

    /// <summary>
    /// Get list of categories
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <returns>List of all categories</returns>
    Task<ObservableCollection<string>> GetCategories(ObservableCollection<TailData> tailData);

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <returns>Task</returns>
    Task WriteXmlFile();

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <param name="tailData">TailData</param>
    /// <returns>Task</returns>
    Task UpdateXmlFile(TailData tailData);

    /// <summary>
    /// Delete XML node from config file
    /// </summary>
    /// <param name="id">Id to remove from XML scheme</param>
    /// <returns>Task</returns>
    Task DeleteXmlElement(string id);

    /// <summary>
    /// Delete a filter element from XML config file
    /// </summary>
    /// <param name="id">Id of parent XML element</param>
    /// <param name="filterId">Id of filter to remove</param>
    /// <returns>Task</returns>
    Task DeleteFilterElement(string id, string filterId);

    /// <summary>
    /// Get XML node by certain Id
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <param name="id">Id</param>
    /// <returns><c>TailData</c>, otherwise <c>Null</c></returns>
    Task<TailData> GetNodeById(ObservableCollection<TailData> tailData, Guid id);
  }
}
