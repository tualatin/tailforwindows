using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Data;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Controller
{
  /// <summary>
  /// XML history controller
  /// </summary>
  public class XmlHistoryController : IXmlSearchHistory<QueueSet<string>>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlHistoryController));

    private static readonly object MyLock = new object();

    private readonly string _historyFile;
    private XDocument _xmlDocument;
    private QueueSet<string> _historyList;

    /// <summary>
    /// Wrap at the end of search: Not implemented!
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlHistoryController() => _historyFile = EnvironmentContainer.TailStorePath + @"\LogfileHistory.xml";

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    public async Task<QueueSet<string>> ReadXmlFileAsync() => await Task.Run(() => ReadXmlFile()).ConfigureAwait(false);

    private QueueSet<string> ReadXmlFile()
    {
      lock ( MyLock )
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
    }

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    public async Task SaveSearchHistoryAsync(string searchWord) => await Task.Run(() => SaveSearchHistory(searchWord)).ConfigureAwait(false);

    private void SaveSearchHistory(string fileName)
    {
      lock ( MyLock )
      {
        LOG.Trace("Save history");

        if ( string.IsNullOrWhiteSpace(fileName) )
          return;

        if ( File.Exists(_historyFile) )
          File.Delete(_historyFile);

        try
        {
          _xmlDocument = new XDocument(new XElement(XmlNames.HistoryXmlRoot));

          if ( _historyList == null )
            _historyList = new QueueSet<string>(SettingsHelperController.CurrentSettings.HistoryMaxSize);

          _historyList.Enqueue(fileName);

          var root = _xmlDocument.Root?.Element(XmlNames.LogfileHistory);

          if ( root == null )
          {
            root = new XElement(XmlNames.LogfileHistory);
            _xmlDocument.Root?.Add(root);
          }

          foreach ( string s in _historyList )
          {
            var find = new XElement(XmlNames.History);
            find.Add(new XAttribute(XmlBaseStructure.Name, s));
            root.Add(find);
          }

          _xmlDocument.Save(_historyFile, SaveOptions.None);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }
    }

    /// <summary>
    /// Save search history wrap as XML attribute: Not implemented!
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    public Task<XElement> SaveSearchHistoryWrapAttributeAsync() => throw new NotImplementedException();

    /// <summary>
    /// Deletes current history
    /// </summary>
    /// <returns>Task</returns>
    public async Task DeleteHistoryAsync() => await Task.Run(() => DeleteHistory()).ConfigureAwait(false);

    private void DeleteHistory()
    {
      lock ( MyLock )
      {
        _historyList?.Clear();

        if ( !File.Exists(_historyFile) )
          return;

        File.Delete(_historyFile);
      }
    }
  }
}
