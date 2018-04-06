using System.Threading.Tasks;
using System.Xml.Linq;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// XML search history interface
  /// </summary>
  /// <typeparam name="T">Type of interface</typeparam>
  public interface IXmlSearchHistory<T>
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
    Task<T> ReadXmlFileAsync();

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
