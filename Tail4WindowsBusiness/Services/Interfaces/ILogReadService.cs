﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Delegates;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.Services.Interfaces
{
  /// <summary>
  /// Log read service interface
  /// </summary>
  public interface ILogReadService
  {
    /// <summary>
    /// A new <see cref="Data.LogEntry"/> is created
    /// </summary>
    event LogEntryCreated OnLogEntryCreated;

    /// <summary>
    /// <see cref="ISmartWatchController"/> current SmartWatch
    /// </summary>
    ISmartWatchController SmartWatch
    {
      get;
    }

    /// <summary>
    /// Size refresh time
    /// </summary>
    string SizeRefreshTime
    {
      get;
    }

    /// <summary>
    /// <see cref="Core.Data.TailData"/>
    /// </summary>
    TailData TailData
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="System.ComponentModel.BackgroundWorker"/> is busy
    /// </summary>
    bool IsBusy
    {
      get;
    }

    /// <summary>
    /// Current log index
    /// </summary>
    int Index
    {
      get;
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    void StartTail();

    /// <summary>
    /// Stops tail
    /// </summary>
    void StopTail();

    /// <summary>
    /// Reset current index
    /// </summary>
    void ResetIndex();

    /// <summary>
    /// Set current index to special value
    /// </summary>
    /// <param name="index">Index</param>
    void SetIndex(int index);

    /// <summary>
    /// Get <see cref="ObservableCollection{T}"/> of <see cref="WindowsEventCategory"/> with Windows events categories
    /// </summary>
    /// <returns>Task</returns>
    Task<ObservableCollection<WindowsEventCategory>> GetCategoriesAsync();
  }
}
