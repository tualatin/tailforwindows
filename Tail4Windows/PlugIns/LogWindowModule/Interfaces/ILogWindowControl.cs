using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// LogWindow interface
  /// </summary>
  public interface ILogWindowControl : INotifyPropertyChanged
  {
    #region Events

    /// <summary>
    /// Lines and time changed event
    /// </summary>
    event LinesRefreshTimeChangedEventHandler OnLinesTimeChanged;

    /// <summary>
    /// On status changed event
    /// </summary>
    event StatusChangedEventHandler OnStatusChanged;

    /// <summary>
    /// On selected lines changed event
    /// </summary>
    event SelectedLinesChangedEventHandler OnSelectedLinesChanged;

    #endregion

    /// <summary>
    /// <see cref="ILogReadService"/>
    /// </summary>
    ILogReadService TailReader
    {
      get;
    }

    /// <summary>
    /// LogWindowTabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    DragSupportTabItem LogWindowTabItem
    {
      get;
      set;
    }

    /// <summary>
    /// Current LogWindowState <see cref="EStatusbarState"/>
    /// </summary>
    EStatusbarState LogWindowState
    {
      get;
      set;
    }

    /// <summary>
    /// Is selected
    /// </summary>
    bool IsSelected
    {
      get;
    }

    /// <summary>
    /// Selected item
    /// </summary>
    string SelectedItem
    {
      get;
      set;
    }

    /// <summary>
    /// Current tail data <see cref="TailData"/>
    /// </summary>
    TailData CurrentTailData
    {
      get;
      set;
    }

    /// <summary>
    /// Current file is valid
    /// </summary>
    bool FileIsValid
    {
      get;
      set;
    }

    /// <summary>
    /// Splitter position
    /// </summary>
    double SplitterPosition
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
    /// IsSmartWatch and Autorun activated
    /// </summary>
    bool IsSmartWatchAutoRun
    {
      get;
      set;
    }

    /// <summary>
    /// SplitWindow control
    /// </summary>
    ISplitWindowControl SplitWindow
    {
      get;
    }

    /// <summary>
    /// Create tail data window
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    void CreateTailDataWindow(TailData item);

    /// <summary>
    /// Open SmartWatch tail data
    /// </summary>
    /// <param name="tailData"><see cref="TailData"/></param>
    void OpenSmartWatchTailData(TailData tailData);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    Task DisposeAsync();

    /// <summary>
    /// Stops current TailReader
    /// </summary>
    void ExecuteStopTailCommand();

    /// <summary>
    /// Set Windows event tail reader
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    void SetWindowsEventTailReader(TailData item);

    /// <summary>
    /// Gets parent window id
    /// </summary>
    Guid ParentWindowId
    {
      get;
    }

    /// <summary>
    /// Window id
    /// </summary>
    Guid WindowId
    {
      get;
    }

    /// <summary>
    /// ImageSource
    /// </summary>
    ImageSource IconSource
    {
      get;
    }
  }
}
