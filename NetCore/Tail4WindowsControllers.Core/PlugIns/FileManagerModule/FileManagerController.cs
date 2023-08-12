using System.Collections.ObjectModel;
using System.IO;
using log4net;
using Org.Vs.Tail4Win.Business.SmartWatchEngine.Controllers;
using Org.Vs.Tail4Win.Business.SmartWatchEngine.Interfaces;
using Org.Vs.Tail4Win.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.Tail4Win.Core.Controllers;
using Org.Vs.Tail4Win.Core.Data;
using Org.Vs.Tail4Win.Core.Logging;
using Org.Vs.Tail4Win.Core.Utils;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.FileManagerModule
{
  /// <summary>
  /// FileManager (for JSON use only) controller
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
      var fileManagerCollection = await _xmlFileManager.ReadXmlFileAsync(token).ConfigureAwait(false);
      return fileManagerCollection == null ||
             fileManagerCollection.Count == 0 ||
             await Task.Run(() => ConvertXmlToJsonConfig(fileManagerCollection), token).ConfigureAwait(false);
    }

    private bool ConvertXmlToJsonConfig(ObservableCollection<TailData> fileManagerCollection)
    {
      try
      {
        LOG.Info("Convert XML file to JSON file");

        JsonUtils.WriteJsonFile(fileManagerCollection, _fileManagerFile);

        if ( File.Exists(_xmlFileManager.XmlFileName) )
          File.Move(_xmlFileManager.XmlFileName, _xmlFileManager.XmlFileName + "_old");

        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Add new tailData JSON file
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    /// <exception cref="ArgumentException">If <paramref name="item"/> is null</exception>
    public async Task<bool> AddTailDataAsync(TailData item, CancellationToken token, ObservableCollection<TailData> tailData = null)
    {
      Arg.NotNull(item, nameof(item));

      if ( tailData == null )
        tailData = await ReadJsonFileAsync(token);

      if ( tailData == null || tailData.Count == 0 )
        tailData = new ObservableCollection<TailData>(new[] { item });
      else
        tailData.Add(item);

      return await CreateUpdateJsonFileAsync(tailData, token);
    }

    /// <summary>
    /// Deletes a <see cref="TailData"/> item by this Id
    /// </summary>
    /// <param name="id">Id of item to delete</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="ObservableCollection{T}"/> if <see cref="TailData"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    /// <exception cref="ArgumentException"> If <paramref name="id"/> is null</exception>
    public async Task<bool> DeleteTailDataByIdAsync(Guid id, CancellationToken token, ObservableCollection<TailData> tailData = null)
    {
      Arg.NotNull(id, nameof(id));

      if ( tailData == null )
        tailData = await ReadJsonFileAsync(token);

      if ( tailData == null || tailData.Count == 0 )
        return false;

      var toDelete = tailData.FirstOrDefault(p => p.Id == id);
      return toDelete != null && tailData.Remove(toDelete) && await CreateUpdateJsonFileAsync(tailData, token);
    }

    /// <summary>
    /// Deletes a filter entry <see cref="FilterData"/>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <param name="tailData"></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    /// <exception cref="ArgumentException"> If <paramref name="id"/> is null</exception>
    public async Task<bool> DeleteFilterDataByIdAsync(Guid id, CancellationToken token, ObservableCollection<TailData> tailData = null)
    {
      Arg.NotNull(id, nameof(id));

      if ( tailData == null )
        tailData = await ReadJsonFileAsync(token);

      if ( tailData == null || tailData.Count == 0 )
        return false;

      var result = false;

      await Task.Run(() =>
      {
        foreach ( TailData data in tailData )
        {
          var filterToDelete = data.ListOfFilter.FirstOrDefault(p => p.Id == id);

          if ( filterToDelete == null )
            continue;

          result = data.ListOfFilter.Remove(filterToDelete);
          break;
        }
      }, token).ConfigureAwait(false);

      return result && await CreateUpdateJsonFileAsync(tailData, token);
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

      return tailData.Count == 0 || await Task.Run(() => CreateUpdateJsonFile(tailData), token).ConfigureAwait(false);
    }

    private bool CreateUpdateJsonFile(ObservableCollection<TailData> tailData)
    {
      LOG.Trace("Create or update JSON db file");

      try
      {
        JsonUtils.WriteJsonFile(tailData, _fileManagerFile);
        return true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
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

      LOG.Trace("Read JSON file");
      var result = await Task.Run(() => JsonUtils.ReadJsonFile<ObservableCollection<TailData>>(_fileManagerFile), token).ConfigureAwait(false);

      if ( result != null && SettingsHelperController.CurrentSettings.SmartWatch )
        await ModifyFileNameBySmartWatchAsync(result);

      if ( result != null )
      {
        await RemoveDuplicateItemsAsync(result, token).ContinueWith(p =>
        {
          foreach ( var data in p.Result )
          {
            data.IsLoadedByXml = true;
          }

          result = p.Result;
        }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(false);
      }
      return result;
    }

    private async Task ModifyFileNameBySmartWatchAsync(IReadOnlyCollection<TailData> result)
    {
      foreach ( TailData item in result.Where(p => p != null && p.SmartWatch && p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameByPatternAsync(item, item.PatternString).ConfigureAwait(false);
      }

      foreach ( var item in result.Where(p => p != null && p.SmartWatch && !p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameBySmartWatchAsync(item).ConfigureAwait(false);
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

      foreach ( var item in grouped )
      {
        w.ListOfFilter.Add(item);
      }
    }

    /// <summary>
    /// Updates a <see cref="TailData"/> item
    /// </summary>
    /// <param name="item"><see cref="TailData"/> to update</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="ObservableCollection{T}"/> if <see cref="TailData"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> UpdateTailDataAsync(TailData item, CancellationToken token, ObservableCollection<TailData> tailData = null)
    {
      if ( item == null )
        return false;

      if ( tailData == null )
        tailData = await ReadJsonFileAsync(token);

      if ( tailData == null || tailData.Count == 0 )
        return false;

      var toChange = tailData.FirstOrDefault(p => p.Id == item.Id);

      if ( toChange == null )
        return false;

      var index = tailData.IndexOf(toChange);
      tailData[index] = item;

      return await CreateUpdateJsonFileAsync(tailData, token);
    }

    /// <summary>
    /// Gets a list of categories from JSON file
    /// <para>Optional you can insert a list of <see cref="IReadOnlyCollection{T}"/> of <see cref="TailData"/></para>
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData"><see cref="IReadOnlyCollection{T}"/> of <see cref="TailData"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="string"/></returns>
    public async Task<ObservableCollection<string>> GetCategoriesAsync(CancellationToken token, IReadOnlyCollection<TailData> tailData = null)
    {
      LOG.Trace("Get all categories from JSON db file");

      if ( tailData == null )
        tailData = await ReadJsonFileAsync(token);

      return tailData == null || tailData.Count == 0 ?
        new ObservableCollection<string>() :
        await Task.Run(() => GetCategories(tailData), token).ConfigureAwait(false);
    }

    private ObservableCollection<string> GetCategories(IReadOnlyCollection<TailData> tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));

      try
      {
        var categories = tailData.Select(p => p.Category).Distinct().ToList();
        var result = new ObservableCollection<string>(categories);

        return result;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
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
      return await Task.Run(() => GetTailDataById(tailData, id), token).ConfigureAwait(false);
    }

    private TailData GetTailDataById(IEnumerable<TailData> tailData, Guid id)
    {
      try
      {
        return tailData.FirstOrDefault(p => p.Id.Equals(id));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return new TailData();
    }
  }
}
