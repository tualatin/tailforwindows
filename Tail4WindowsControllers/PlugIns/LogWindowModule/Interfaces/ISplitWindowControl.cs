﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces
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
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    ObservableCollection<LogEntry> FindWhatResults
    {
      get;
    }

    /// <summary>
    /// LogReaderService
    /// </summary>
    ILogReadService LogReaderService
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
    /// Selected text in ListBox
    /// </summary>
    string SelectedText
    {
      get;
    }

    /// <summary>
    /// <see cref="List{T}"/> of <see cref="MessageFloodData"/>
    /// </summary>
    List<MessageFloodData> FloodData
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="ListCollectionView"/> of <see cref="LogEntry"/>
    /// </summary>
    ListCollectionView CollectionView
    {
      get;
      set;
    }

    /// <summary>
    /// CacheManager interface
    /// </summary>
    ICacheManager CacheManager
    {
      get;
      set;
    }

    /// <summary>
    /// GoToLine
    /// </summary>
    /// <param name="index">Index</param>
    void GoToLine(int index);

    /// <summary>
    /// Clears current items
    /// </summary>
    void ClearItems();
  }
}