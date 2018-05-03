﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Events.Args;
using Org.Vs.TailForWin.Business.Events.Delegates;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.Business.Services
{
  /// <summary>
  /// Log read service
  /// </summary>
  public class LogReadService : ILogReadService
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogReadService));

    private CancellationToken _token;
    private int _startOffset;

    #region Events

    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    public event LogEntryCreated OnLogEntryCreated;

    #endregion

    #region Properties

    /// <summary>
    /// Lines read
    /// </summary>
    public int LinesRead
    {
      get;
      private set;
    }

    /// <summary>
    /// Size and refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
      private set;
    }

    /// <summary>
    /// <see cref="Core.Data.TailData"/>
    /// </summary>
    public TailData TailData
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogReadService()
    {
      _startOffset = SettingsHelperController.CurrentSettings.LinesRead;
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    public async Task StartTailAsync(CancellationToken token)
    {
      LOG.Trace("Start tail...");

      Thread.CurrentThread.Priority = TailData.ThreadPriority;
      _token = token;

      await ReadLogFileAsync().ConfigureAwait(false);
    }

    private async Task ReadLogFileAsync()
    {
      int index = 1;

      while ( !_token.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromMilliseconds((int) TailData.RefreshRate), _token).ConfigureAwait(false);

        var log = new LogEntry
        {
          Index = index,
          Message = $"Log - {index}",
          DateTime = DateTime.Now
        };

        LinesRead = index;
        SizeRefreshTime = string.Format(Application.Current.TryFindResource("SizeRefreshTime").ToString(), 123, DateTime.Now);

        OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(log, LinesRead, SizeRefreshTime));
        index++;
      }
    }

    /// <summary>
    /// Stop tail
    /// </summary>
    public void StopTail()
    {
      MouseService.SetBusyState();
      LOG.Trace("Stop tail.");
    }
  }
}