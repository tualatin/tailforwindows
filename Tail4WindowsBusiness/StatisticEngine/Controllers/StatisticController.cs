using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Org.Vs.TailForWin.Core.Collections;
using Org.Vs.TailForWin.Core.Controllers;
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

    private readonly SemaphoreSlim _myLock = new SemaphoreSlim(1);
    private readonly SemaphoreSlim _updateLock = new SemaphoreSlim(1);

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

      NotifyTaskCompletion.Create(CreateSessionEntryAsync);

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

      try
      {
        await SaveAllValuesIntoDatabaseAsync();
        _cts.Cancel();
      }
      finally
      {
        IsBusy = false;
      }
    }

    /// <summary>
    /// Starts analysis
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="StatisticData"/></returns>
    public async Task<IStatisticAnalysisCollection<StatisticAnalysisData>> StartAnalysisAsync(CancellationToken token)
    {
      LOG.Debug("Start statistics analysis");

      IStatisticAnalysisCollection<StatisticAnalysisData> result = new StatisticAnalysisCollection();

      await _myLock.WaitAsync(token).ConfigureAwait(false);
      await Task.Run(() =>
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            var sessionEntity = GetSessionEntity(db);
            var fileEntity = GetFileEntity(db);
            var existsSessions = sessionEntity.FindAll();

            Parallel.ForEach(existsSessions, new ParallelOptions { CancellationToken = token }, session =>
              {
                var files = fileEntity.FindAll().Where(p => p.Session.SessionId == session.SessionId).ToList();

                if ( !files.Any() )
                  return;

                var temp = new StatisticAnalysisData { SessionEntity = session, Files = files };
                result.Add(temp);
              });
          }

          result.OrderCollectionByDate();
        }
        finally
        {
          _myLock.Release();
        }
      }, token).ConfigureAwait(false);

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
        LogCount = Convert.ToUInt64(data.Index),
        FileName = data.FileName,
        ElapsedTime = data.ElapsedTime,
        IsWindowsEvent = data.IsWindowsEvent,
        BookmarkCount = Convert.ToUInt64(data.BookmarkCount),
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

    private Task AddFileToCurrentSessionAsync(StatisticData data)
    {
      AddFileToQueue(data);
      return StartFileQueueAsync();
    }

    private async Task StartFileQueueAsync()
    {
      await _myLock.WaitAsync(_cts.Token).ConfigureAwait(false);

      try
      {
        using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
        {
          var sessionEntity = GetSessionEntity(db);
          var existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);

          while ( existsSession == null )
          {
            await Task.Delay(DelayTimeInMs).ConfigureAwait(false);
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

    private Task WorkingQueueAsync() =>
      Task.Run(() =>
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            var sessionEntity = GetSessionEntity(db);
            var fileEntity = GetFileEntity(db);
            var existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);

            if ( existsSession == null )
              return;

            while ( _fileQueue.Peek() != null )
            {
              var temp = _fileQueue.Dequeue();
              var file = fileEntity
                           .Include(p => p.Session)
                           .FindAll()
                           .FirstOrDefault(p => p.Session.Session == SessionId && (p.LogReaderId == temp.LogReaderId || temp.FileName == p.FileName)) ?? new FileEntity { FileName = temp.FileName, LogReaderId = temp.LogReaderId, Session = existsSession, IsWindowsEvent = temp.IsWindowsEvent };
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
          _myLock.Release();
        }
      }, _cts.Token);

    private async Task SaveAllValuesIntoDatabaseAsync()
    {
      await _myLock.WaitAsync(_cts.Token).ConfigureAwait(false);
      await Task.Run(() =>
      {
        UpdateSession();

        try
        {
          var upTime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);

          if ( upTime.Hours < 1 )
          {
            using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
            {
              var sessionEntity = GetSessionEntity(db);
              var fileEntity = GetFileEntity(db);
              var existsSession = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId);

              if ( existsSession != null )
              {
                LOG.Debug($"Remove existing session from DataBase {SessionId}");
                RemoveFiles(fileEntity, existsSession);
                sessionEntity.DeleteMany(p => p.Session == SessionId);
                db.Rebuild();
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
              var invalidFiles = result.Where(p => p.ElapsedTime == null || p.ElapsedTime != null && TimeSpan.Compare(p.ElapsedTime.Value, minElapsedTime) < 0).ToList();

              foreach ( var file in invalidFiles )
              {
                fileEntity.DeleteMany(p => p.FileId == file.FileId);
              }

              RemoveSessionIfRequired(fileEntity, sessionEntity);
            }

            long shrinkSize = db.Rebuild();
            LOG.Debug($"Database shrink: {shrinkSize}");
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          _myLock.Release();
        }
      }, _cts.Token).ConfigureAwait(false);
    }

    private async Task CreateSessionEntryAsync()
    {
      await RemoveOldSessionsAsync();
      await RemoveInvalidSessionsAsync();

      while ( !_cts.IsCancellationRequested )
      {
        UpdateSession();
        await Task.Delay(TimeSpan.FromMinutes(30), _cts.Token).ConfigureAwait(false);
      }
    }

    private void UpdateSession()
    {
      _updateLock.Wait(_cts.Token);

      try
      {
        LOG.Debug("Insert / update current session");

        long value = GC.GetTotalMemory(false);

        using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
        {
          var sessionEntity = GetSessionEntity(db);
          var session = sessionEntity.FindAll().FirstOrDefault(p => p.Session == SessionId) ?? new SessionEntity
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
        _updateLock.Release();
      }
    }

    private async Task RemoveOldSessionsAsync()
    {
      await _myLock.WaitAsync(_cts.Token).ConfigureAwait(false);
      await Task.Run(() =>
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
                sessionEntity.DeleteMany(p => p.Session == session.Session);
              }
              else
              {
                valid = true;
              }
            }

            db.Rebuild();
          }
        }
        finally
        {
          _myLock.Release();
        }
      }, _cts.Token).ConfigureAwait(false);
    }

    private async Task RemoveInvalidSessionsAsync()
    {
      await _myLock.WaitAsync(_cts.Token).ConfigureAwait(false);
      await Task.Run(() =>
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
            var t4W = new List<Process>();

            if ( !SettingsHelperController.CurrentSettings.SingleInstance )
            {
              var allProcesses = Process.GetProcesses();
              t4W = allProcesses.Where(p => p.MainWindowTitle == CoreEnvironment.ApplicationTitle).ToList();

              foreach ( var session in sessions )
              {
                if ( t4W.Count > 1 )
                {
                  // If not single instance, do not remove this sessions, because it's possible, it's a running session
                  if ( DateTime.Now.Date == session.Date.Date )
                    continue;
                }

                RemoveFiles(fileEntity, session);
                sessionEntity.DeleteMany(p => p.Session == session.Session);
              }
            }
            else
            {
              foreach ( var session in sessions )
              {
                RemoveFiles(fileEntity, session);
                sessionEntity.DeleteMany(p => p.Session == session.Session);
              }
            }

            // Remove all sessions without files
            var files = fileEntity.Include(p => p.Session).FindAll().Select(p => p.Session.Session).Distinct().ToList();
            sessions = sessionEntity.FindAll().ToList();

            if ( files.Count == 0 )
            {
              foreach ( var session in sessions )
              {
                sessionEntity.DeleteMany(p => p.Session == session.Session);
              }
            }
            else
            {
              var result = sessions.Where(p => files.All(x => x != p.Session)).ToList();

              foreach ( var session in result )
              {
                if ( t4W.Any() )
                {
                  // If not single instance, do not remove this sessions, because it's possible, it's a running session
                  if ( DateTime.Now.Date == session.Date.Date )
                    continue;
                }

                sessionEntity.DeleteMany(p => p.Session == session.Session);
              }
            }

            t4W.Clear();

            // Remove files without elapsed time
            var invalidFiles = fileEntity.FindAll().Where(p => p.ElapsedTime == null).ToList();

            if ( invalidFiles.Count == 0 )
              return;

            foreach ( var file in invalidFiles )
            {
              fileEntity.DeleteMany(p => p.FileId == file.FileId);
            }
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          _myLock.Release();
        }
      }, _cts.Token).ConfigureAwait(false);
    }

    private List<FileEntity> RemoveSessionIfRequired(ILiteCollection<FileEntity> fileEntity, ILiteCollection<SessionEntity> sessionEntity)
    {
      try
      {
        // Remove session without files
        var result = fileEntity.Include(p => p.Session).FindAll().Where(p => p.Session.Session == SessionId).ToList();

        if ( result.Count == 0 )
        {
          LOG.Debug($"Remove existing session from DataBase {SessionId}");
          sessionEntity.DeleteMany(p => p.Session == SessionId);
        }
        return result;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return new List<FileEntity>();
    }

    private void RemoveFiles(ILiteCollection<FileEntity> fileEntity, SessionEntity session)
    {
      try
      {
        var result = fileEntity.Include(p => p.Session).FindAll().Where(p => p.Session.Session == session?.Session).ToList();

        foreach ( var file in result )
        {
          fileEntity.DeleteMany(p => p.FileId == file.FileId);
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        var result = fileEntity.Include(p => p.Session).FindAll().Where(p => p.Session == null).ToList();

        foreach ( var file in result )
        {
          fileEntity.DeleteMany(p => p.FileId == file.FileId);
        }
      }
    }

    private ILiteCollection<SessionEntity> GetSessionEntity(LiteDatabase db)
    {
      var sessionEntity = db.GetCollection<SessionEntity>(StatisticEnvironment.SessionEntityName);
      return sessionEntity;
    }

    private ILiteCollection<FileEntity> GetFileEntity(LiteDatabase db)
    {
      var fileEntity = db.GetCollection<FileEntity>(StatisticEnvironment.FileEntityName);
      return fileEntity;
    }

    #endregion
  }
}
