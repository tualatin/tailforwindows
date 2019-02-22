using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Data;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FindModule
{
  /// <summary>
  /// XML history controller
  /// </summary>
  [Obsolete("Please use History controller instead")]
  public class XmlSearchHistoryController : IXmlSearchHistory<IObservableDictionary<string, string>>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlSearchHistoryController));
    private static readonly object MyLock = new object();

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    private readonly string _historyFile;
    private XDocument _xmlDocument;

    /// <summary>
    /// Wrap at the end of search
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlSearchHistoryController() => _historyFile = CoreEnvironment.UserSettingsPath + @"\History.xml";

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlSearchHistoryController(string path) => _historyFile = path;

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    public async Task<IObservableDictionary<string, string>> ReadXmlFileAsync() => await Task.Run(() => ReadXmlFile());

    private IObservableDictionary<string, string> ReadXmlFile()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          var concurrentDictionary = new ConcurrentDictionary<string, string>();

          if ( !File.Exists(_historyFile) )
            return new ObservableDictionary<string, string>();

          LOG.Trace("Read search history");

          _xmlDocument = XDocument.Load(_historyFile);
          XElement historyRoot = _xmlDocument.Root?.Element(XmlNames.FindHistory);

          if ( historyRoot == null )
            return null;

          string wrap = historyRoot.Attribute(XmlNames.Wrap)?.Value;
          Wrap = wrap.ConvertToBool();

          Parallel.ForEach(historyRoot.Elements(XmlNames.Find), f =>
          {
            // ReSharper disable once AssignNullToNotNullAttribute
            concurrentDictionary.TryAdd(f.Attribute(XmlBaseStructure.Name)?.Value, f.Attribute(XmlBaseStructure.Name)?.Value);
          });

          return new ObservableDictionary<string, string>(concurrentDictionary);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }

      LOG.Error("Can not lock!");
      return null;
    }
  }
}
