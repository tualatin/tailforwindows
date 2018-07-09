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

    private static readonly object MyLock = new object();

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
      LOG.Info("Start statistics");

      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      NotifyTaskCompletion.Create(GetUsedMemoryAsync);

      IsBusy = true;
    }

    /// <summary>
    /// Stops statistics
    /// </summary>
    public async Task StopAsync()
    {
      MouseService.SetBusyState();

      LOG.Info("Stop statistics");

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
      LOG.Trace("Start statistics analysis");

      var result = new StatisticData();

      lock ( MyLock )
      {

      }
      return result;
    }

    #region HelperFunctions

    private async Task SaveAllValuesIntoDatabaseAsync()
    {
      await Task.Run(() =>
      {
        lock ( MyLock )
        {
          TimeSpan uptime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);

          if ( uptime.Hours < 1 )
          {
            LOG.Info($"Statistics not saved, {EnvironmentContainer.ApplicationTitle} was active less than 1 hour: {uptime.Minutes} minute(s)!");
            return;
          }

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
