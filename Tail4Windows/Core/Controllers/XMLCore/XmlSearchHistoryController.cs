using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Org.Vs.TailForWin.Core.Interfaces.XmlCore;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers.XmlCore
{
  /// <summary>
  /// XML history controller
  /// </summary>
  public class XmlSearchHistoryController : IXmlSearchHistory
  {
    /// <summary>
    /// Read XML file
    /// </summary>
    /// <param name="history"></param>
    /// <returns>Task</returns>
    public Task ReadXmlFileAsync(ref ObservableDictionary<string, string> history)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    public Task SaveSearchHistoryAsync(string searchWord)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Save search history wrap as XML attribute
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    public Task<XElement> SaveSearchHistoryWrapAttributeAsync()
    {
      throw new NotImplementedException();
    }
  }
}
