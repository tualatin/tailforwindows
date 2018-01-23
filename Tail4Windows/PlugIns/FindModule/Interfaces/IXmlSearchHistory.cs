using System.Threading.Tasks;
using System.Xml.Linq;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.FindModule.Interfaces
{
  /// <summary>
  /// XML search history interface
  /// </summary>
  public interface IXmlSearchHistory
  {
    /// <summary>
    /// Wrap at the end of search
    /// </summary>
    bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    Task<IObservableDictionary<string, string>> ReadXmlFileAsync();

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
