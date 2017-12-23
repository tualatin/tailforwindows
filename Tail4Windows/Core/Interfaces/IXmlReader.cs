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
    /// <returns></returns>
    Task<ObservableCollection<TailData>> ReadXmlFile();

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <returns></returns>
    Task WriteXmlFile();

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <returns></returns>
    Task UpdateXmlFile();

    /// <summary>
    /// Delete XML node from config file
    /// </summary>
    /// <returns></returns>
    Task DeleteXmlElement();

    /// <summary>
    /// Get XML node by certain Id
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns><c>TailData</c>, otherwise <c>Null</c></returns>
    Task<TailData> GetNodeById(string id);
  }
}
