using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Controllers;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule
{
  /// <summary>
  /// FileManager controller
  /// </summary>
  public class FileManagerController : IFileManagerController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileManagerController));
    private static readonly object MyLock = new object();

    private readonly string _fileManagerFile;
    private readonly ISmartWatchController _smartWatchController;
    private readonly IXmlFileManager _xmlFileManager;

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FileManagerController()
    {
      _fileManagerFile = CoreEnvironment.UserSettingsPath + @"\FileManager.json";
      _smartWatchController = new SmartWatchController();
      _xmlFileManager = new XmlFileManagerController();
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="jsonPath">Path of JSON file</param>
    /// <param name="path">Path of XML file</param>
    public FileManagerController(string jsonPath, string path)
    {
      _fileManagerFile = jsonPath;
      _xmlFileManager = new XmlFileManagerController(path);
      _smartWatchController = new SmartWatchController();
    }

    /// <summary>
    /// Converts old XML config file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ConvertXmlToJsonConfigAsync(CancellationToken token)
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          var fileManagerCollection = await _xmlFileManager.ReadXmlFileAsync(token);

          if ( fileManagerCollection == null || fileManagerCollection.Count == 0 )
            return true;

          using ( FileStream fs = File.Open(_fileManagerFile, FileMode.OpenOrCreate) )
          using ( var sw = new StreamWriter(fs) )
          using ( JsonWriter jw = new JsonTextWriter(sw) )
          {
            jw.Formatting = Formatting.Indented;
            var serializer = new JsonSerializer();
            serializer.Serialize(jw, fileManagerCollection);
          }

          if ( File.Exists(_xmlFileManager.XmlFileName) )
            File.Delete(_xmlFileManager.XmlFileName);

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
      }
      else
      {
        LOG.Error("Can not lock!");
      }

      return false;
    }

    /// <summary>
    /// Reads a JSON file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>List of tail settings from JSON file</returns>
    public async Task<ObservableCollection<TailData>> ReadJsonFileAsync(CancellationToken token)
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          if ( !File.Exists(_fileManagerFile) )
            return new ObservableCollection<TailData>();

          var result = await Task.Run(() => ReadJsonFile(), token);

          if ( result != null && SettingsHelperController.CurrentSettings.SmartWatch )
            await ModifyFileNameBySmartWatchAsync(result);

          if ( result != null )
            result = await RemoveDuplicateItemsAsync(result, token);

          return result;
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }

      LOG.Error("Can not lock!");

      return new ObservableCollection<TailData>();
    }

    private ObservableCollection<TailData> ReadJsonFile()
    {
      using ( StreamReader sr = File.OpenText(_fileManagerFile) )
      {
        var serializer = new JsonSerializer();
        var json = (List<TailData>) serializer.Deserialize(sr, typeof(List<TailData>));

        return new ObservableCollection<TailData>(json);
      }
    }

    private async Task ModifyFileNameBySmartWatchAsync(ObservableCollection<TailData> result)
    {
      foreach ( TailData item in result.Where(p => p != null && p.SmartWatch && p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameByPatternAsync(item, item.PatternString);
      }

      foreach ( TailData item in result.Where(p => p != null && p.SmartWatch && !p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameBySmartWatchAsync(item);
      }
    }

    private async Task<ObservableCollection<TailData>> RemoveDuplicateItemsAsync(ObservableCollection<TailData> items, CancellationToken token)
    {
      LOG.Trace("Try to remove duplicate items");

      ObservableCollection<TailData> result = null;

      await Task.Run(() =>
      {
        try
        {
          var list = new List<TailData>();

          // Group all items not IsWindowsEvent
          list.AddRange(items.Where(p => p != null && !p.IsWindowsEvent).GroupBy(p => p.FileName.ToLower()).Select(p => p.FirstOrDefault()).ToList());
          list.ForEach(InsertFilterData);

          // Group all item IsWindowsEvent
          list.AddRange(items.Where(p => p != null && p.IsWindowsEvent).GroupBy(p => p.File.ToLower()).Select(p => p.FirstOrDefault()).ToList());
          list.ForEach(InsertFilterData);

          result = new ObservableCollection<TailData>(list);
        }
        catch
        {
          result = items;
        }
      }, token).ConfigureAwait(false);

      return result;
    }

    private static void InsertFilterData(TailData w)
    {
      var grouped = w.ListOfFilter.GroupBy(p => p.Filter.ToLower()).Select(p => p.FirstOrDefault()).ToList();
      w.ListOfFilter.Clear();

      foreach ( FilterData item in grouped )
      {
        w.ListOfFilter.Add(item);
      }
    }
  }
}
