using System.Threading.Tasks;
using System.Xml.Linq;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Interfaces.XMLCore
{
  /// <summary>
  /// XML search history interface
  /// 
  /// </summary>
  public interface IXmlSearchHistory
  {
    /// <summary>
    /// Read XML file
    /// </summary>
    /// <param name="history"></param>
    /// <returns>Task</returns>
    Task ReadXmlFileAsync(ref ObservableDictionary<string ,string> history);

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    Task SaveSearchHistoryAsync(string searchWord);

    /// <summary>
    /// Save search history wrap as XML attribute
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    Task<XElement> SaveSearchHistoryWrapAttributeAsync();
  }
}
