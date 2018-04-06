using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Controller
{
  /// <summary>
  /// XML history controller
  /// </summary>
  public class XmlHistoryController : IXmlSearchHistory<QueueSet<string>> 
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlHistoryController));

    private const int MaxQueueSize = 15;

    /// <summary>
    /// Wrap at the end of search: Not implemented!
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    public async Task<QueueSet<string>> ReadXmlFileAsync()
    {
      LOG.Trace("Read history");

      return await Task.Run(() => ReadXmlFile()).ConfigureAwait(false);
    }

    private QueueSet<string> ReadXmlFile()
    {
      var history = new QueueSet<string>(MaxQueueSize);

      return history;
    }

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    public async Task SaveSearchHistoryAsync(string searchWord)
    {
    }

    /// <summary>
    /// Save search history wrap as XML attribute: Not implemented!
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    public Task<XElement> SaveSearchHistoryWrapAttributeAsync()
    {
      throw new System.NotImplementedException();
    }
  }
}
