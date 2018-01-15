using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Interfaces.XmlCore;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers.XmlCore
{
  /// <summary>
  /// XML history controller
  /// </summary>
  public class XmlSearchHistoryController : IXmlSearchHistory
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlSearchHistoryController));

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
    public XmlSearchHistoryController()
    {
      _historyFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\FileManager.xml";
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlSearchHistoryController(string path)
    {
      _historyFile = path;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    public async Task<IObservableDictionary<string, string>> ReadXmlFileAsync()
    {
      if ( !File.Exists(_historyFile) )
        throw new FileNotFoundException();

      LOG.Trace("Read search history");

      return await Task.Run(() => ReadXmlFile()).ConfigureAwait(false);
    }

    private IObservableDictionary<string, string> ReadXmlFile()
    {
      IObservableDictionary<string, string> history = new ObservableDictionary<string, string>();

      try
      {
        _xmlDocument = XDocument.Load(_historyFile);
        var historyRoot = _xmlDocument.Root?.Element(XmlStructure.FindHistory);

        if ( historyRoot == null )
          return null;

        string wrap = historyRoot.Attribute(XmlStructure.Wrap)?.Value;
        Wrap = bool.TryParse(wrap, out bool wrapHistory) && wrapHistory;
        Parallel.ForEach(historyRoot.Elements(XmlStructure.Find), f => history.Add(f.Attribute(XmlStructure.Name)?.Value, f.Attribute(XmlStructure.Name)?.Value));

        return history;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return null;
    }

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    public async Task SaveSearchHistoryAsync(string searchWord) => await Task.Run(() => SaveSearchHistory(searchWord)).ConfigureAwait(false);

    private void SaveSearchHistory(string word)
    {
      LOG.Trace("Save search history");

      if ( string.IsNullOrWhiteSpace(word) )
        return;

      if (!File.Exists(_historyFile))
        _xmlDocument = new XDocument(new XElement(XmlStructure.HistoryXmlRoot));
      
      try
      {
        var root = _xmlDocument.Root?.Element(XmlStructure.FindHistory) ?? SaveSearchHistoryWrapAttribute();
        var find = new XElement(XmlStructure.Find);
        find.Add(new XAttribute(XmlStructure.Name, word));
        root.Add(find);

        _xmlDocument.Save(_historyFile, SaveOptions.None);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }

    /// <summary>
    /// Save search history wrap as XML attribute
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    public async Task<XElement> SaveSearchHistoryWrapAttributeAsync() => await Task.Run(() => SaveSearchHistoryWrapAttribute());

    private XElement SaveSearchHistoryWrapAttribute()
    {
      LOG.Trace("Update wrap attribute in search history");

      if ( !File.Exists(_historyFile) )
        _xmlDocument = new XDocument(new XElement(XmlStructure.HistoryXmlRoot));

      try
      {
        XElement root = _xmlDocument.Root?.Element(XmlStructure.FindHistory);

        if ( root != null )
        {
          root.Attribute(XmlStructure.Wrap)?.SetValue(Wrap.ToString());
        }
        else
        {
          root = new XElement(XmlStructure.FindHistory);
          root.Add(new XAttribute(XmlStructure.Wrap, Wrap.ToString()));
          _xmlDocument.Root?.Add(root);
        }

        _xmlDocument.Save(_historyFile, SaveOptions.None);
        return root;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return null;
    }
  }
}
