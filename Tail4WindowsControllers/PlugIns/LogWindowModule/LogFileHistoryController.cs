using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule
{
  /// <summary>
  /// Log file history controller
  /// </summary>
  public class LogFileHistoryController : IHistory<LogFileHistoryData>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogFileHistoryController));
    private readonly SemaphoreSlim _semaphore;
    private readonly string _historyFile;
    private readonly IXmlSearchHistory<QueueSet<string>> _xmlHistoryController;
    private readonly string _xmlHistoryFile;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogFileHistoryController()
    {
      _semaphore = new SemaphoreSlim(1, 1);
      _xmlHistoryController = new XmlHistoryController();
      _historyFile = CoreEnvironment.UserSettingsPath + @"\LogfileHistory.json";
      _xmlHistoryFile = CoreEnvironment.UserSettingsPath + @"\LogfileHistory.xml";
    }

    /// <summary>
    /// Converts old XML history file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ConvertXmlToJsonFileAsync(CancellationToken token)
    {
      await _semaphore.WaitAsync(token);

      var history = await _xmlHistoryController.ReadXmlFileAsync();

      if ( history == null || history.Count == 0 )
        return true;

      LOG.Info("Convert XML file to JSON file");

      try
      {
        return await Task.Run(() => ConvertXmlToJsonFile(history), token);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private bool ConvertXmlToJsonFile(QueueSet<string> history)
    {
      try
      {
        var historyData = new LogFileHistoryData();

        foreach ( var name in history )
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
      return false;
    }

    /// <summary>
    /// Reads history file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="HistoryData"/></returns>
    public async Task<LogFileHistoryData> ReadHistoryAsync(CancellationToken token)
    {
      if ( !File.Exists(_historyFile) )
        return new LogFileHistoryData();

      await _semaphore.WaitAsync(token);
      LOG.Trace("Read JSON file");

      try
      {
        return await Task.Run(() => JsonUtils.ReadJsonFile<LogFileHistoryData>(_historyFile), token);
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
    public async Task<bool> UpdateHistoryAsync(LogFileHistoryData data, string searchText, CancellationToken token)
    {
      await _semaphore.WaitAsync(token);
      LOG.Trace("Update history");

      try
      {
        return await Task.Run(() => UpdateHistory(data, searchText.Trim()), token);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private bool UpdateHistory(LogFileHistoryData data, string searchText)
    {
      try
      {
        if ( !string.IsNullOrWhiteSpace(searchText) )
        {
          if ( !data.FindCollection.Contains(searchText) && !IsInvalidChars(searchText) )
            data.FindCollection.Add(searchText);
        }

        JsonUtils.WriteJsonFile(data, _historyFile);

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Deletes the history
    /// </summary>
    /// <param name="data">Data to delete</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> DeleteHistoryAsync(LogFileHistoryData data, CancellationToken token)
    {
      await _semaphore.WaitAsync(token);
      LOG.Trace("Delete history");

      try
      {
        return await Task.Run(() => DeleteHistory(data), token);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private bool DeleteHistory(LogFileHistoryData data)
    {
      try
      {
        data.FindCollection.Clear();
        JsonUtils.WriteJsonFile(data, _historyFile);

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return false;
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
