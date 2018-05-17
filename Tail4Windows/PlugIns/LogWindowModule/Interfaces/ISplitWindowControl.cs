using System.Collections.Generic;
using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// SplitWindowControl interface
  /// </summary>
  public interface ISplitWindowControl
  {
    /// <summary>
    /// Current height
    /// </summary>
    double CurrentHeight
    {
      get;
      set;
    }

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    TailData CurrentTailData
    {
      get;
      set;
    }

    /// <summary>
    /// Lines read
    /// </summary>
    int LinesRead
    {
      get;
    }

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    ObservableCollection<LogEntry> LogEntries
    {
      get;
      set;
    }

    /// <summary>
    /// LogReaderService
    /// </summary>
    LogReadService LogReaderService
    {
      get;
      set;
    }

    /// <summary>
    /// Current splitter height
    /// </summary>
    double SplitterPosition
    {
      get;
      set;
    }

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index
    /// </summary>
    int LastVisibleLogEntryIndex
    {
      get;
      set;
    }

    /// <summary>
    /// Selected <see cref="LogEntry"/> item
    /// </summary>
    LogEntry SelectedItem
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="LogEntry"/> cache
    /// </summary>
    List<LogEntry> EntryCache
    {
      get;
      set;
    }
  }
}
