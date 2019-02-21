using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FindModule
{
  /// <summary>
  /// History controller
  /// </summary>
  public class HistoryController : IHistory
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(HistoryController));
    private static readonly object MyLock = new object();

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    private readonly string _historyFile;
    private readonly IXmlSearchHistory<IObservableDictionary<string, string>> _xmlHistoryController;
    private readonly string _xmlHistoryFile;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public HistoryController()
    {
      _xmlHistoryController = new XmlSearchHistoryController();
      _historyFile = CoreEnvironment.UserSettingsPath + @"\History.json";
      _xmlHistoryFile = CoreEnvironment.UserSettingsPath + @"\History.xml";
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="jsonPath">Path of JSON file</param>
    /// <param name="path">Path of XML file</param>
    public HistoryController(string jsonPath, string path)
    {
      _xmlHistoryController = new XmlSearchHistoryController(path);
      _xmlHistoryFile = path;
      _historyFile = jsonPath;
    }

    /// <summary>
    /// Converts old XML history file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ConvertXmlToJsonFileAsync(CancellationToken token)
    {
      var history = await _xmlHistoryController.ReadXmlFileAsync();

      if ( history == null || history.Count == 0 )
        return true;

      if ( !Monitor.TryEnter(MyLock, LockTimeSpanIsMs) )
        return false;

      LOG.Info("Convert XML file to JSON file");

      try
      {
        var historyData = new HistoryData
        {
          Wrap = _xmlHistoryController.Wrap
        };

        foreach ( var name in history.Keys )
        {
          historyData.FindCollection.Add(name);
        }

        JsonUtils.WriteJsonFile(historyData, _historyFile);

        if ( File.Exists(_xmlHistoryFile) )
          File.Move(_xmlHistoryFile, _xmlHistoryFile + "_old");

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      finally
      {
        Monitor.Exit(MyLock);
      }
      return false;
    }

    /// <summary>
    /// Reads history file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="HistoryData"/></returns>
    public async Task<HistoryData> ReadHistoryAsync(CancellationToken token)
    {
      if ( !File.Exists(_historyFile) )
        return new HistoryData();


      if ( !Monitor.TryEnter(MyLock, LockTimeSpanIsMs) )
        return new HistoryData();

      LOG.Trace("Read JSON file");

      try
      {
        var result = await Task.Run(() => JsonUtils.ReadJsonFile<HistoryData>(_historyFile), token);

        return result;
      }
      finally
      {
        Monitor.Exit(MyLock);
      }
    }

    /// <summary>
    /// Saves a search text to history file
    /// </summary>
    /// <param name="searchText">Text to save</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> SaveHistoryAsync(string searchText, CancellationToken token)
    {
      if ( !Monitor.TryEnter(MyLock, LockTimeSpanIsMs) )
        return false;

      LOG.Trace("Save history");

      try
      {
        var history = await Task.Run(() => JsonUtils.ReadJsonFile<HistoryData>(_historyFile), token);

        history.FindCollection.Add(searchText);
        JsonUtils.WriteJsonFile(history, _historyFile);

        return true;
      }
      finally
      {
        Monitor.Exit(MyLock);
      }
    }
  }
}
