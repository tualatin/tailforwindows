using System.IO;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Logging;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.TailForWin.Controllers.PlugIns.FindModule
{
  /// <summary>
  /// History controller
  /// </summary>
  public class HistoryController : IHistory<HistoryData>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(HistoryController));
    private readonly SemaphoreSlim _semaphore;
    private readonly string _historyFile;
    private readonly IXmlSearchHistory<IObservableDictionary<string, string>> _xmlHistoryController;
    private readonly string _xmlHistoryFile;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public HistoryController()
    {
      _semaphore = new SemaphoreSlim(1, 1);
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
      _semaphore = new SemaphoreSlim(1, 1);
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
      await _semaphore.WaitAsync(token);

      var history = await _xmlHistoryController.ReadXmlFileAsync().ConfigureAwait(false);

      if ( history == null || history.Count == 0 )
        return true;

      LOG.Info("Convert XML file to JSON file");

      try
      {
        return await Task.Run(() => ConvertXmlToJsonFile(history), token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private bool ConvertXmlToJsonFile(IObservableDictionary<string, string> history)
    {
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
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
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

      await _semaphore.WaitAsync(token);
      LOG.Trace("Read JSON file");

      try
      {
        return await Task.Run(() => JsonUtils.ReadJsonFile<HistoryData>(_historyFile), token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    /// <summary>
    /// Updates history file
    /// </summary>
    /// <param name="data">Data to update</param>
    /// <param name="searchText">Text to save</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> UpdateHistoryAsync(HistoryData data, string searchText, CancellationToken token)
    {
      await _semaphore.WaitAsync(token);
      LOG.Trace("Update history");

      try
      {
        return await Task.Run(() => UpdateHistory(data, searchText.Trim()), token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private bool UpdateHistory(HistoryData data, string searchText)
    {
      try
      {
        if ( !string.IsNullOrWhiteSpace(searchText) )
        {
          if ( !data.FindCollection.Contains(searchText) )
            data.FindCollection.Add(searchText);
        }

        JsonUtils.WriteJsonFile(data, _historyFile);

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Deletes the history
    /// </summary>
    /// <param name="data">Data to delete</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> DeleteHistoryAsync(HistoryData data, CancellationToken token)
    {
      await _semaphore.WaitAsync(token);
      LOG.Trace("Delete history");

      try
      {
        return await Task.Run(() => DeleteHistory(data), token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private bool DeleteHistory(HistoryData data)
    {
      try
      {
        data.FindCollection.Clear();
        JsonUtils.WriteJsonFile(data, _historyFile);

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return false;
    }
  }
}
