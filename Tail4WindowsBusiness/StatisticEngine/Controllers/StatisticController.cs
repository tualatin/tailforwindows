using System;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.StatisticEngine.Controllers
{
  /// <summary>
  /// Statistics controller
  /// </summary>
  public class StatisticController : IStatisticController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(StatisticController));

    private CancellationTokenSource _cts;

    #region Properties

    /// <summary>
    /// Statistics is busy
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    #endregion

    /// <summary>
    /// Starts statistics
    /// </summary>
    public void Start()
    {
      LOG.Info("Starts statistics");

      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

      NotifyTaskCompletion.Create(GetUsedMemoryAsync);

      IsBusy = true;
    }

    /// <summary>
    /// Stops statistics
    /// </summary>
    public async Task StopAsync()
    {
      MouseService.SetBusyState();
      LOG.Info("Stops statistics");

      await SaveAllValuesIntoDatabaseAsync().ConfigureAwait(false);
      _cts.Cancel();

      IsBusy = false;
    }

    /// <summary>
    /// Starts analysis
    /// </summary>
    /// <returns><see cref="StatisticData"/></returns>
    public async Task<StatisticData> StartAnalysisAsync()
    {
      LOG.Trace("Starts statistics analysis");

      var result = new StatisticData();

      return result;
    }

    #region HelperFunctions

    private async Task SaveAllValuesIntoDatabaseAsync()
    {
      await Task.Run(() =>
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            long shrinkSize = db.Shrink();
            LOG.Trace($"Database shrink: {shrinkSize}");
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }, _cts.Token).ConfigureAwait(false);
    }

    private async Task GetUsedMemoryAsync()
    {
      while ( !_cts.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromHours(1), _cts.Token).ConfigureAwait(false);

        long value = GC.GetTotalMemory(false);
        LOG.Trace($"Write current used memory {value:N0} into database");
      }
    }

    #endregion
  }
}
