using System.Collections.Generic;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl.Interfaces
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
    /// Last visible <see cref="Business.Services.Data.LogEntry"/> index property
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
    /// Text editor search highlight background
    /// </summary>
    string TextEditorSearchHighlightBackgroundHex
    {
      get;
      set;
    }

    /// <summary>
    /// Text editor search highlight foreground
    /// </summary>
    string TextEditorSearchHighlightForegroundHex
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
  }
}
