using System;
using System.Collections.Generic;
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
    /// Delay time in milliseconds
    /// </summary>
    private const int DelayTimeInMs = 100;

    /// <summary>
    /// Max elements in DataBase
    /// </summary>
    private const int MaxDataBaseElements = 100;

    private QueueSet<FileEntity> _fileQueue;
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
      _fileQueue = new QueueSet<FileEntity>(int.MaxValue);

      NotifyTaskCompletion.Create(CreateSessionEntry);

      IsBusy = true;
    }

    /// <summary>
    /// Starts to dequeue the file queue
    /// </summary>
    public void StartFileQueue() => NotifyTaskCompletion.Create(StartFileQueueAsync);

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
    public async Task<StatisticAnalysisData> StartAnalysisAsync()
    {
      LOG.Debug("Start statistics analysis");

      var result = new StatisticAnalysisData();

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

    /// <summary>
    /// Adds file to current session
    /// </summary>
    /// <param name="data">Data as <see cref="StatisticData"/></param>
    public void AddFileToCurrentSession(StatisticData data) => NotifyTaskCompletion.Create(AddFileToCurrentSessionAsync(data));

    /// <summary>
    /// Adds file to current queue
    /// </summary>
    /// <param name="data">Data as <see cref="StatisticData"/></param>
    public void AddFileToQueue(StatisticData data) =>
      _fileQueue.Enqueue(new FileEntity
      {
        LogReaderId = data.LogReaderId,
        LogCount = data.Index,
        FileName = data.FileName,
        ElapsedTime = data.ElapsedTime,
        IsWindowsEvent = data.IsWindowsEvent,
        BookmarkCount = data.BookmarkCount,
        FileSizeTotalEvents = data.FileSizeTotalEvents
      });

    /// <summary>
    /// Saves file to current session
    /// </summary>
    /// <param name="data">Data as <see cref="StatisticData"/></param>
    public void SaveFileToCurrentSession(StatisticData data)
    {
      AddFileToQueue(data);
      NotifyTaskCompletion.Create(StartFileQueueAsync);
    }

    #region HelperFunctions

    private async Task AddFileToCurrentSessionAsync(StatisticData data)
    {
      AddFileToQueue(data);
      await StartFileQueueAsync();
    }

    private async Task StartFileQueueAsync()
    {
      while ( Monitor.IsEntered(MyLock) )
      {
        await Task.Delay(DelayTimeInMs);
      }

      try
      {
        using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
        {
          var sessionEntity = GetSessionEntity(db);
          SessionEntity existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);

          while ( existsSession == null )
          {
            await Task.Delay(DelayTimeInMs);
            existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);
          }
        }

        await WorkingQueueAsync();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private async Task WorkingQueueAsync() => await Task.Run(() =>
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            var sessionEntity = GetSessionEntity(db);
            var fileEntity = GetFileEntity(db);
            SessionEntity existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);

            if ( existsSession == null )
              return;

            while ( _fileQueue.Peek() != null )
            {
              var temp = _fileQueue.Dequeue();
              FileEntity file = fileEntity
                                  .Include(p => p.Session)
                                  .FindAll()
                                  .FirstOrDefault(p => p.Session.Session == SessionId && (p.LogReaderId == temp.LogReaderId || temp.FileName == p.FileName)) ?? new FileEntity
                                  {
                                    FileName = temp.FileName,
                                    LogReaderId = temp.LogReaderId,
                                    Session = existsSession,
                                    IsWindowsEvent = temp.IsWindowsEvent
                                  };
              file.LogCount = temp.LogCount;
              file.BookmarkCount = temp.BookmarkCount;
              file.FileSizeTotalEvents = temp.FileSizeTotalEvents;

              if ( temp.LogReaderId == file.LogReaderId )
              {
                if ( string.CompareOrdinal(temp.FileName, file.FileName) != 0 )
                  file.IsSmartWatch = true;
              }

              if ( temp.ElapsedTime.HasValue )
                file.ElapsedTime = temp.ElapsedTime;

              fileEntity.Upsert(file);
              fileEntity.EnsureIndex(p => p.FileName);
            }
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

    private async Task SaveAllValuesIntoDatabaseAsync() => await Task.Run(() =>
    {
      UpdateSession();

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
              var fileEntity = GetFileEntity(db);
              SessionEntity existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);

              if ( existsSession != null )
              {
                LOG.Debug($"Remove existing session from DataBase {SessionId}");
                RemoveFiles(fileEntity, existsSession);
                sessionEntity.Delete(p => p.Session == SessionId);
                db.Shrink();
              }
            }

            LOG.Info($"Statistics not saved, {CoreEnvironment.ApplicationTitle} was active less than 1 hour: {upTime.Minutes} minute(s)!");
            return;
          }

          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            var sessionEntity = GetSessionEntity(db);
            var fileEntity = GetFileEntity(db);
            var result = RemoveSessionIfRequired(fileEntity, sessionEntity);

            if ( result.Count > 0 )
            {
              // Remove files with low elapsed time, minimum 15 min!
              var minElapsedTime = new TimeSpan(0, 0, 15, 0);
              var invalidFiles = result.Where(p => p.ElapsedTime != null && TimeSpan.Compare(p.ElapsedTime.Value, minElapsedTime) < 0).ToList();

              foreach ( var file in invalidFiles )
              {
                fileEntity.Delete(p => p.FileId == file.FileId);
              }

              RemoveSessionIfRequired(fileEntity, sessionEntity);
            }

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

    private async Task CreateSessionEntry()
    {
      await RemoveOldSessionsAsync();
      await RemoveInvalidSessionsAsync();

      while ( !_cts.IsCancellationRequested )
      {
        UpdateSession();
        await Task.Delay(TimeSpan.FromMinutes(30), _cts.Token);
      }
    }

    private void UpdateSession()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Debug("Insert / update current session");

          long value = GC.GetTotalMemory(false);

          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            var sessionEntity = GetSessionEntity(db);
            SessionEntity session = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId) ?? new SessionEntity
            {
              Session = SessionId,
              Date = DateTime.Now
            };
            session.MemoryUsage = value;
            session.UpTime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);

            sessionEntity.Upsert(session);
            sessionEntity.EnsureIndex(p => p.Session);
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
    }

    private async Task RemoveOldSessionsAsync() => await Task.Run(() =>
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
              var fileEntity = GetFileEntity(db);

              var sessions = sessionEntity.FindAll().ToList();

              if ( sessions.Count > MaxDataBaseElements )
              {
                var session = sessions.FirstOrDefault(p => p.Date == sessions.Min(d => d.Date));
                RemoveFiles(fileEntity, session);
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

    private async Task RemoveInvalidSessionsAsync() => await Task.Run(() =>
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            LOG.Debug("Remove invalid sessions from DataBase");

            var sessionEntity = GetSessionEntity(db);
            var fileEntity = GetFileEntity(db);

            // Minimum is 1 hour!
            var minUpTime = new TimeSpan(0, 1, 0, 0);
            var sessions = sessionEntity.FindAll().Where(p => TimeSpan.Compare(p.UpTime, minUpTime) < 0).ToList();

            foreach ( SessionEntity session in sessions )
            {
              RemoveFiles(fileEntity, session);
              sessionEntity.Delete(p => p.Session == session.Session);
            }

            // Remove all sessions without files
            var files = fileEntity.Include(p => p.Session).FindAll().Select(p => p.Session.Session).Distinct().ToList();
            sessions = sessionEntity.FindAll().ToList();

            if ( files.Count == 0 )
            {
              foreach ( SessionEntity session in sessions )
              {
                sessionEntity.Delete(p => p.Session == session.Session);
              }
            }
            else
            {
              var result = sessions.Where(p => files.All(x => x != p.Session)).ToList();

              foreach ( SessionEntity session in result )
              {
                sessionEntity.Delete(p => p.Session == session.Session);
              }
            }
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

    private List<FileEntity> RemoveSessionIfRequired(LiteCollection<FileEntity> fileEntity, LiteCollection<SessionEntity> sessionEntity)
    {
      try
      {
        // Remove session without files
        var result = fileEntity.Include(p => p.Session).FindAll().Where(p => p.Session.Session == SessionId).ToList();

        if ( result.Count == 0 )
        {
          LOG.Debug($"Remove existing session from DataBase {SessionId}");
          sessionEntity.Delete(p => p.Session == SessionId);
        }
        return result;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return new List<FileEntity>();
    }

    private void RemoveFiles(LiteCollection<FileEntity> fileEntity, SessionEntity session)
    {
      try
      {
        var result = fileEntity.Include(p => p.Session).FindAll().Where(p => p.Session.Session == session?.Session).ToList();

        foreach ( var file in result )
        {
          fileEntity.Delete(p => p.FileId == file.FileId);
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        var result = fileEntity.Include(p => p.Session).FindAll().Where(p => p.Session == null).ToList();

        foreach ( var file in result )
        {
          fileEntity.Delete(p => p.FileId == file.FileId);
        }
      }
    }

    private LiteCollection<SessionEntity> GetSessionEntity(LiteDatabase db)
    {
      var sessionEntity = db.GetCollection<SessionEntity>(StatisticEnvironment.SessionEntityName);
      return sessionEntity;
    }

    private LiteCollection<FileEntity> GetFileEntity(LiteDatabase db)
    {
      var fileEntity = db.GetCollection<FileEntity>(StatisticEnvironment.FileEntityName);
      return fileEntity;
    }

    #endregion
  }
}
