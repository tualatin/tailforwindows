﻿using System.Collections.ObjectModel;
using System.Windows;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.Core.Collections.FilterCollections;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// SplitWindowControl interface
  /// </summary>
  public interface ISplitWindowControl
  {

    #region Properties

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
    }

    /// <summary>
    /// Global filter collection
    /// </summary>
    ObservableCollection<FilterData> GlobalFilters
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
    /// Selected lines
    /// </summary>
    int SelectedLines
    {
      get;
    }

    /// <summary>
    /// SelectedSplitSearch item
    /// </summary>
    string SelectedSplitSearchItem
    {
      get;
    }

    /// <summary>
    /// Search history
    /// </summary>
    HistoryData SearchHistory
    {
      get;
    }

    /// <summary>
    /// LogCollectionView <see cref="VsCollectionView{T}"/>
    /// </summary>
    IVsCollectionView<LogEntry> LogCollectionView
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
    /// Highlight data <see cref="List{T}"/> of <see cref="TextHighlightData"/>
    /// </summary>
    List<TextHighlightData> HighlightData
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
    /// Extended toolbar visibility
    /// </summary>
    Visibility ExtendedToolbarVisibility
    {
      get;
      set;
    }

    /// <summary>
    /// TextBox has focus
    /// </summary>
    bool TextBoxHasFocus
    {
      get;
    }

    /// <summary>
    /// Filtered by Bookmark
    /// </summary>
    bool SplitElementFilterByBookmark
    {
      get;
      set;
    }

    /// <summary>
    /// Filtered by Text
    /// </summary>
    string SplitElementFilterText
    {
      get;
      set;
    }

    /// <summary>
    /// Gets current Bookmark count
    /// </summary>
    int BookmarkCount
    {
      get;
    }

    #endregion

    /// <summary>
    /// Updates <see cref="TailData"/>"
    /// </summary>
    /// <param name="tailData"><see cref="TailData"/></param>
    void UpdateTailData(TailData tailData);

    /// <summary>
    /// GoToLine
    /// </summary>
    /// <param name="index">Index</param>
    void GoToLine(int index);

    /// <summary>
    /// Clears current items
    /// </summary>
    void ClearItems();

    /// <summary>
    /// Unregister FindWhat changed or closed message
    /// </summary>
    void UnregisterFindWhatChanged();

    /// <summary>
    /// Initialize the CollectionView
    /// </summary>
    void InitCollectionView();
  }
}
