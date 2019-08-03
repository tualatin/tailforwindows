using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// Global filter controller
  /// </summary>
  public class GlobalFilterController : IGlobalFilterController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(GlobalFilterController));

    private readonly SemaphoreSlim _semaphore;
    private readonly string _globalFilterFile;

    /// <summary>
    /// Constructor
    /// </summary>
    public GlobalFilterController()
    {
      _semaphore = new SemaphoreSlim(1, 1);
      _globalFilterFile = CoreEnvironment.UserSettingsPath + @"\GlobalFilter.json";
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="jsonPath">Path of JSON file</param>
    public GlobalFilterController(string jsonPath)
    {
      _semaphore = new SemaphoreSlim(1, 1);
      _globalFilterFile = jsonPath;
    }

    /// <summary>
    /// Deletes a filter async
    /// </summary>
    /// <param name="id">ID of filter to delete</param>
    /// <returns><c>True</c> if successfully deleted, otherwise <c>False</c></returns>
    public async Task<bool> DeleteFilterAsync(Guid id)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Loads filters async
    /// </summary>
    /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="FilterData"/></returns>
    public async Task<ObservableCollection<FilterData>> LoadFiltersAsync()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Save filter async
    /// </summary>
    /// <param name="items">Global filters to save</param>
    public async Task SaveFilterAsync(ObservableCollection<FilterData> items)
    {
      Arg.NotNull(items, nameof(items));

      await _semaphore.WaitAsync();

      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
      {
        await Task.Run(() =>
        {
          try
          {
            if ( items.Count == 0 )
              return;

            JsonUtils.WriteJsonFile(items, _globalFilterFile);
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
          finally
          {
            _semaphore.Release();
          }
        }, cts.Token).ConfigureAwait(false);
      }
    }
  }
}
