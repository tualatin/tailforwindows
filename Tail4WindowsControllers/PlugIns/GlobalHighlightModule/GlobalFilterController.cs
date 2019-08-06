using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Controllers.PlugIns.GlobalHighlightModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.GlobalHighlightModule
{
  /// <summary>
  /// Global filter controller
  /// </summary>
  public class GlobalFilterController : IGlobalFilterController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(GlobalFilterController));

    /// <summary>
    /// Item does not exists
    /// </summary>
    public const string ItemDoesNotExists = "Item does not exists";

    /// <summary>
    /// Global highlight list is empty
    /// </summary>
    public const string GlobalHighlightListIsEmpty = "Global highlight list is empty";

    private readonly SemaphoreSlim _semaphore;
    private readonly SemaphoreSlim _semaphoreDelete;
    private readonly string _globalFilterFile;

    /// <summary>
    /// Constructor
    /// </summary>
    public GlobalFilterController()
    {
      _semaphore = new SemaphoreSlim(1, 1);
      _semaphoreDelete = new SemaphoreSlim(1, 1);
      _globalFilterFile = CoreEnvironment.UserSettingsPath + @"\GlobalFilter.json";
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="jsonPath">Path of JSON file</param>
    public GlobalFilterController(string jsonPath)
    {
      _semaphore = new SemaphoreSlim(1, 1);
      _semaphoreDelete = new SemaphoreSlim(1, 1);
      _globalFilterFile = jsonPath;
    }

    /// <summary>
    /// Deletes a filter async
    /// </summary>
    /// <param name="id">ID of filter to delete</param>
    /// <returns><c>True</c> if successfully deleted, otherwise <c>False</c></returns>
    /// <exception cref="ArgumentException">if id is null</exception>
    public async Task<bool> DeleteGlobalFilterAsync(Guid id)
    {
      if ( id == Guid.Empty )
        throw new ArgumentException(nameof(id));

      if ( !File.Exists(_globalFilterFile) )
        return false;

      await _semaphoreDelete.WaitAsync().ConfigureAwait(false);

      try
      {
        using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
        {
          var items = await ReadGlobalFiltersAsync(cts.Token);

          if ( items.Count == 0 )
            throw new Exception(GlobalHighlightListIsEmpty);

          var toDelete = items.FirstOrDefault(p => p.Id == id);

          if ( toDelete == null )
            throw new ArgumentOutOfRangeException(nameof(id), ItemDoesNotExists);

          var result = items.Remove(toDelete);

          await UpdateGlobalFilterAsync(items);

          return result;
        }
      }
      finally
      {
        _semaphoreDelete.Release();
      }
    }

    /// <summary>
    /// Loads filters async
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="FilterData"/></returns>
    public async Task<ObservableCollection<FilterData>> ReadGlobalFiltersAsync(CancellationToken token)
    {
      if ( !File.Exists(_globalFilterFile) )
        return new ObservableCollection<FilterData>();

      await _semaphore.WaitAsync(token).ConfigureAwait(false);
      LOG.Trace("Read JSON file");

      try
      {
        return await Task.Run(() => JsonUtils.ReadJsonFile<ObservableCollection<FilterData>>(_globalFilterFile), token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    /// <summary>
    /// Save filter async
    /// </summary>
    /// <param name="items">Global filters to save</param>
    /// <returns><c>True</c> if successfully deleted, otherwise <c>False</c></returns>
    /// <exception cref="ArgumentException">If items is null</exception>
    public async Task<bool> UpdateGlobalFilterAsync(ObservableCollection<FilterData> items)
    {
      Arg.NotNull(items, nameof(items));

      await _semaphore.WaitAsync().ConfigureAwait(false);
      LOG.Trace("Update global filters");

      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
      {
        var result = false;

        await Task.Run(() =>
        {
          try
          {
            if ( items.Count == 0 )
              return result;

            FixFilterToGlobal(items);
            JsonUtils.WriteJsonFile(items, _globalFilterFile);
            result = true;
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
          finally
          {
            _semaphore.Release();
          }
          return result;
        }, cts.Token).ConfigureAwait(false);
      }
      return false;
    }

    private void FixFilterToGlobal(ObservableCollection<FilterData> items)
    {
      foreach ( var item in items.Where(p => !p.IsGlobal).ToArray() )
      {
        item.IsGlobal = true;
      }
    }
  }
}
