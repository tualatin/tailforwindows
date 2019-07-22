using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Collections;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule
{
  /// <summary>
  /// XML file history controller
  /// </summary>
  [Obsolete("Please use LogFileHistoryController instead")]
  public class XmlHistoryController : IXmlSearchHistory<QueueSet<string>>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlHistoryController));

    private static readonly object MyLock = new object();

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    private readonly string _historyFile;
    private XDocument _xmlDocument;
    private QueueSet<string> _historyList;

    /// <summary>
    /// Wrap at the end of search: Not implemented!
    /// </summary>
    public bool Wrap
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlHistoryController() => _historyFile = CoreEnvironment.UserSettingsPath + @"\LogfileHistory.xml";

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    public async Task<QueueSet<string>> ReadXmlFileAsync() => await Task.Run(() => ReadXmlFile());

    private QueueSet<string> ReadXmlFile()
    {
      if ( Monitor.TryEnter(MyLock, LockTimeSpanIsMs) )
      {
        try
        {
          if ( !File.Exists(_historyFile) )
            return new QueueSet<string>(SettingsHelperController.CurrentSettings.HistoryMaxSize);

          LOG.Trace("Read history");

          _historyList = new QueueSet<string>(SettingsHelperController.CurrentSettings.HistoryMaxSize);

          try
          {
            _xmlDocument = XDocument.Load(_historyFile);
            var historyRoot = _xmlDocument.Root?.Element(XmlNames.LogfileHistory);

            if ( historyRoot == null )
              return _historyList;

            foreach ( var f in historyRoot.Elements(XmlNames.History) )
            {
              _historyList.Add(f.Attribute(XmlBaseStructure.Name)?.Value);
            }
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }

          return _historyList;
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      return new QueueSet<string>(SettingsHelperController.CurrentSettings.HistoryMaxSize);
    }
  }
}
