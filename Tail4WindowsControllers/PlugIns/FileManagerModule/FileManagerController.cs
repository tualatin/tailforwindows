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

    private readonly string _fileManagerFile;
    private readonly ISmartWatchController _smartWatchController;
    private readonly IXmlFileManager _xmlFileManager;


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
      var fileManagerCollection = await _xmlFileManager.ReadXmlFileAsync(token);

      if ( fileManagerCollection == null || fileManagerCollection.Count == 0 )
        return true;

      try
      {
        LOG.Trace("Convert old XML file to JSON db file");

        WriteJsonFile(fileManagerCollection);

        if ( File.Exists(_xmlFileManager.XmlFileName) )
          File.Delete(_xmlFileManager.XmlFileName);

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }

      return false;
    }

    private void WriteJsonFile(ObservableCollection<TailData> fileManagerCollection)
    {
      using ( FileStream fs = File.Open(_fileManagerFile, FileMode.OpenOrCreate) )
      using ( var sw = new StreamWriter(fs) )
      using ( JsonWriter jw = new JsonTextWriter(sw) )
      {
        jw.Formatting = Formatting.Indented;
        var serializer = new JsonSerializer
        {
          NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Serialize(jw, fileManagerCollection);
      }
    }

    /// <summary>
    /// Updates a JSON file
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    /// <exception cref="ArgumentException">If <paramref name="tailData"/> is null</exception>
    public async Task<bool> CreateUpdateJsonFileAsync(ObservableCollection<TailData> tailData, CancellationToken token)
    {
      Arg.NotNull(tailData, nameof(tailData));

      return tailData.Count == 0 || await Task.Run(() => CreateUpdateJsonFile(tailData), token);
    }

    private bool CreateUpdateJsonFile(ObservableCollection<TailData> tailData)
    {
      LOG.Trace("Create or update JSON db file");

      try
      {
        WriteJsonFile(tailData);

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }

      return false;
    }

    /// <summary>
    /// Reads a JSON file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></returns>
    public async Task<ObservableCollection<TailData>> ReadJsonFileAsync(CancellationToken token)
    {
      if ( !File.Exists(_fileManagerFile) )
        return new ObservableCollection<TailData>();

      LOG.Trace("Read JSON db file");
      var result = await Task.Run(() => ReadJsonFile(), token);

      if ( result != null && SettingsHelperController.CurrentSettings.SmartWatch )
        await ModifyFileNameBySmartWatchAsync(result);

      if ( result != null )
        result = await RemoveDuplicateItemsAsync(result, token);

      return result;
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

    private async Task ModifyFileNameBySmartWatchAsync(IReadOnlyCollection<TailData> result)
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

    /// <summary>
    /// Gets a list of categories from JSON file
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="string"/></returns>
    /// <exception cref="ArgumentException">If <paramref name="tailData"/> is null</exception>
    public async Task<ObservableCollection<string>> GetCategoriesAsync(ObservableCollection<TailData> tailData, CancellationToken token)
    {
      Arg.NotNull(tailData, nameof(tailData));

      LOG.Trace("Get all categories from JSON db file");
      return await Task.Run(() => GetCategories(tailData), token);
    }

    private ObservableCollection<string> GetCategories(IEnumerable<TailData> tailData)
    {
      try
      {
        var categories = tailData.Select(p => p.Category).Distinct().ToList();
        var result = new ObservableCollection<string>(categories);

        return result;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return new ObservableCollection<string>();
    }

    /// <summary>
    /// Get <c><see cref="TailData"/></c> by certain Id
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="id">Id</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><c><see cref="TailData"/></c>, otherwise <c>Null</c></returns>
    /// <exception cref="ArgumentException">If <paramref name="tailData"/> is null or <paramref name="id"/> is <see cref="Guid.Empty"/></exception>
    public async Task<TailData> GetTailDataByIdAsync(ObservableCollection<TailData> tailData, Guid id, CancellationToken token)
    {
      Arg.NotNull(tailData, nameof(tailData));

      if ( id == Guid.Empty )
        throw new ArgumentException();

      LOG.Trace("Get TailData by '{0}", id);
      return await Task.Run(() => GetTailDataById(tailData, id), token);
    }

    private TailData GetTailDataById(IEnumerable<TailData> tailData, Guid id)
    {
      try
      {
        return tailData.FirstOrDefault(p => p.Id.Equals(id));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return new TailData();
    }
  }
}
