﻿using System.Windows;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.LogWindowUserControl.Interfaces
{
  /// <summary>
  /// LogWindow ListBox interface
  /// </summary>
  public interface ILogWindowListBox
  {
    #region Events

    /// <summary>
    /// Clears ItemsSource event
    /// </summary>
    event RoutedEventHandler ClearItemsEvent;

    /// <summary>
    /// Add Bookmark comment event
    /// </summary>
    event RoutedEventHandler AddBookmarkCommentEvent;

    /// <summary>
    /// Selection of lines changed event
    /// </summary>
    event RoutedEventHandler SelectedLinesChangedEvent;

    #endregion

    #region Properties

    /// <summary>
    /// SelectedText
    /// </summary>
    string SelectedText
    {
      get;
    }

    /// <summary>
    /// <see cref="CurrentTailData"/>
    /// </summary>
    TailData CurrentTailData
    {
      get;
      set;
    }

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index property
    /// </summary>
    int LastVisibleLogEntryIndex
    {
      get;
      set;
    }

    /// <summary>
    /// ShowGridSplitControl
    /// </summary>
    bool ShowGridSplitControl
    {
      get;
      set;
    }

    /// <summary>
    /// Bookmark image size
    /// </summary>
    double BookmarkImageSize
    {
      get;
      set;
    }

    /// <summary>
    /// Highlight data result <see cref="List{T}"/> of <see cref="TextHighlightData"/>
    /// </summary>
    List<TextHighlightData> HighlightDataResult
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Go to item by index
    /// </summary>
    /// <param name="index">Index to go</param>
    void GoToItemByIndex(int index);

    /// <summary>
    /// Scroll to the beginning of list
    /// </summary>
    void ScrollToHome();

    /// <summary>
    /// Scroll to end of list
    /// </summary>
    void ScrollToEnd();

    /// <summary>
    /// Get ViewPort height
    /// </summary>
    /// <returns>ViewPort height, otherwise <see cref="double.NaN"/></returns>
    double GetViewportHeight();

    /// <summary>
    /// Gets scroll viewer VerticalOffset
    /// </summary>
    /// <returns>ScrollViewer VerticalOffset, otherwise <see cref="double.NaN"/></returns>
    double GetScrollViewerVerticalOffset();

    /// <summary>
    /// Update highlighting in <see cref="System.Windows.Controls.TextBlock"/>
    /// </summary>
    /// <param name="result"></param>
    void UpdateHighlighting(List<TextHighlightData> result);
  }
}
