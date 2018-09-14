using System;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Business.Utils;
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

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    /// <summary>
    /// Max elements in DataBase
    /// </summary>
    private const int MaxDataBaseElements = 100;

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

      await SaveAllValuesIntoDatabaseAsync();
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

      await Task.Run(() =>
      {
        if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
        {
          try
          {
          }
          finally
          {
            Monitor.Exit(MyLock);
          }
        }

        LOG.Error("Can not lock!");
      }, _cts.Token);

      return result;
    }

    #region HelperFunctions

    private async Task SaveAllValuesIntoDatabaseAsync()
    {
      await Task.Run(() =>
      {
        if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
        {
          TimeSpan uptime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);

          if ( uptime.Hours < 1 )
          {
            LOG.Info($"Statistics not saved, {CoreEnvironment.ApplicationTitle} was active less than 1 hour: {uptime.Minutes} minute(s)!");
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
          finally
          {
            Monitor.Exit(MyLock);
          }
        }

        LOG.Error("Can not lock!");
      }, _cts.Token);
    }

    private async Task GetUsedMemoryAsync()
    {
      while ( !_cts.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromHours(1), _cts.Token);

        long value = GC.GetTotalMemory(false);
        LOG.Trace($"Write current used memory {value:N0} into database");
      }
    }

    #endregion
  }
}
