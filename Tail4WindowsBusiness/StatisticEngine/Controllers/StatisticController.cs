using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.DbScheme;
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
    private BsonMapper _mapper;

    #region Entity constants

    /// <summary>
    /// SessionEntity
    /// </summary>
    private const string SessionEntityName = "Sessions";

    /// <summary>
    /// FileEntity
    /// </summary>
    private const string FileEntityName = "Files";

    #endregion

    #region Properties

    /// <summary>
    /// Statistics is busy
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    /// <summary>
    /// Current Session ID
    /// </summary>
    public Guid SessionId
    {
      get;
    } = Guid.NewGuid();

    #endregion

    /// <summary>
    /// Starts statistics
    /// </summary>
    public void Start()
    {
      LOG.Info("Start statistics");

      _cts?.Dispose();
      _cts = new CancellationTokenSource();
      _mapper = BsonMapper.Global;

      NotifyTaskCompletion.Create(CreateSessionEntry);

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
      LOG.Debug("Start statistics analysis");

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
        else
        {
          LOG.Error("Can not lock!");
        }
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
          try
          {
            TimeSpan upTime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);

            if ( upTime.Hours < 1 )
            {
              using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
              {
                var sessionEntity = GetSessionEntity(db);
                SessionEntity existsSession = sessionEntity.Find(p => p.Session == SessionId).FirstOrDefault();

                if ( existsSession != null )
                {
                  LOG.Debug($"Remove existing session from DataBase {SessionId}");
                  sessionEntity.Delete(p => p.Session == SessionId);
                  db.Shrink();
                }
              }

              LOG.Info($"Statistics not saved, {CoreEnvironment.ApplicationTitle} was active less than 1 hour: {upTime.Minutes} minute(s)!");
              return;
            }

            using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
            {
              // TODO remove sessions without file list
              long shrinkSize = db.Shrink();
              LOG.Debug($"Database shrink: {shrinkSize}");
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
        else
        {
          LOG.Error("Can not lock!");
        }
      }, _cts.Token);
    }

    private async Task CreateSessionEntry()
    {
      await RemoveOldSessionsAsync();
      await RemoveInvalidSessionsAsync();

      while ( !_cts.IsCancellationRequested )
      {
        if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
        {
          try
          {
            long value = GC.GetTotalMemory(false);

            using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
            {
              var sessionEntity = GetSessionEntity(db);
              SessionEntity session = sessionEntity.Find(p => p.Session == SessionId).FirstOrDefault() ?? new SessionEntity
              {
                Date = DateTime.Now,
                MemoryUsage = value,
                Session = SessionId,
                UpTime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime)
              };

              sessionEntity.EnsureIndex(p => p.Id);
              sessionEntity.Upsert(session);
            }

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

        await Task.Delay(TimeSpan.FromMinutes(20), _cts.Token);
      }
    }

    private async Task RemoveOldSessionsAsync()
    {
      await Task.Run(() =>
      {
        if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
        {
          try
          {
            using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
            {
              LOG.Debug("Remove old sessions from DataBase");

              bool valid = false;

              while ( !valid )
              {
                var sessionEntity = GetSessionEntity(db);
                var sessions = sessionEntity.FindAll().ToList();

                if ( sessions.Count > MaxDataBaseElements )
                {
                  var session = sessions.FirstOrDefault(p => p.Date == sessions.Min(d => d.Date));
                  sessionEntity.Delete(p => p.Session == session.Session);
                }
                else
                {
                  valid = true;
                }
              }

              db.Shrink();
            }
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
      }, _cts.Token);
    }

    private async Task RemoveInvalidSessionsAsync()
    {
      await Task.Run(() =>
      {
        if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
        {
          try
          {
            using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
            {
              LOG.Debug("Remove invalid sessions from DataBase");
              // TODO sessions without file list!

              var sessionEntity = GetSessionEntity(db);

              // Min is 1 hour!
              var minUpTime = new TimeSpan(0, 1, 0, 0);
              var sessions = sessionEntity.Find(p => TimeSpan.Compare(p.UpTime, minUpTime) < 0).ToList();

              foreach ( SessionEntity session in sessions )
              {
                sessionEntity.Delete(p => p.Session == session.Session);
              }

              db.Shrink();
            }
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

      }, _cts.Token);
    }

    private static LiteCollection<SessionEntity> GetSessionEntity(LiteDatabase db)
    {
      var sessionEntity = db.GetCollection<SessionEntity>(SessionEntityName);
      return sessionEntity;
    }

    #endregion
  }
}
