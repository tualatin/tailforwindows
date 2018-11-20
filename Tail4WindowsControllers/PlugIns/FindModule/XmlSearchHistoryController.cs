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

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    public async Task SaveSearchHistoryAsync(string searchWord) => await Task.Run(() => SaveSearchHistory(searchWord));

    private void SaveSearchHistory(string word)
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace("Save search history");

          if ( string.IsNullOrWhiteSpace(word) )
            return;

          if ( !File.Exists(_historyFile) )
            _xmlDocument = new XDocument(new XElement(XmlNames.HistoryXmlRoot));

          XElement root = _xmlDocument.Root?.Element(XmlNames.FindHistory) ?? SaveSearchHistoryWrapAttribute();
          var find = new XElement(XmlNames.Find);
          find.Add(new XAttribute(XmlBaseStructure.Name, word.Trim()));
          root.Add(find);

          _xmlDocument.Save(_historyFile, SaveOptions.None);
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
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    /// <summary>
    /// Save search history wrap as XML attribute
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    public async Task<XElement> SaveSearchHistoryWrapAttributeAsync() => await Task.Run(() => SaveSearchHistoryWrapAttribute());

    private XElement SaveSearchHistoryWrapAttribute()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace("Update wrap attribute in search history");

          if ( !File.Exists(_historyFile) )
            _xmlDocument = new XDocument(new XElement(XmlNames.HistoryXmlRoot));

          XElement root = _xmlDocument.Root?.Element(XmlNames.FindHistory);

          if ( root != null )
          {
            root.Attribute(XmlNames.Wrap)?.SetValue(Wrap.ToString());
          }
          else
          {
            root = new XElement(XmlNames.FindHistory);

            root.Add(new XAttribute(XmlNames.Wrap, Wrap.ToString()));
            _xmlDocument.Root?.Add(root);
          }

          _xmlDocument.Save(_historyFile, SaveOptions.None);
          return root;
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

    /// <summary>
    /// Deletes current history
    /// </summary>
    /// <returns>Task</returns>
    public async Task DeleteHistoryAsync()
    {
      var history = await ReadXmlFileAsync();
      await Task.Run(() =>
      {
        DeleteHistory(history);
      }, new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token);
    }

    private void DeleteHistory(IObservableDictionary<string, string> history)
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          history?.Clear();

          if ( !File.Exists(_historyFile) )
            return;

          if ( IsInvalidChars(_historyFile) )
          {
            InteractionService.ShowErrorMessageBox("Invalid characters found in path or file name.");
            return;
          }

          File.Delete(_historyFile);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    private bool IsInvalidChars(string fileName)
    {
      var invalidFileNameChars = Path.GetInvalidFileNameChars();
      var invalidPathChars = Path.GetInvalidPathChars();
      string path = Path.GetFullPath(fileName);

      return fileName.IndexOfAny(invalidFileNameChars) < 0 && path.IndexOfAny(invalidPathChars) < 0;
    }
  }
}
