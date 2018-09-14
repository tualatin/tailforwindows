using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Utils
{
  /// <summary>
  /// LogWindowControl dummy -> use it for temporary copy interface data
  /// </summary>
  public class LogWindowControlDummy : ILogWindowControl
  {
    #region Events

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Lines and time changed event
    /// </summary>
    public event LinesRefreshTimeChangedEventHandler OnLinesTimeChanged;

    /// <summary>
    /// On status changed event
    /// </summary>
    public event StatusChangedEventHandler OnStatusChanged;

    #endregion

    /// <summary>
    /// <see cref="ILogReadService"/>
    /// </summary>
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public ILogReadService TailReader
    {
      get;
    }

    /// <summary>
    /// LogWindowTabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem LogWindowTabItem
    {
      get;
      set;
    }

    /// <summary>
    /// Current LogWindowState <see cref="EStatusbarState"/>
    /// </summary>
    public EStatusbarState LogWindowState
    {
      get;
      set;
    }

    /// <summary>
    /// Is selected
    /// </summary>
    public bool IsSelected => throw new NotImplementedException();

    /// <summary>
    /// Selected item
    /// </summary>
    public string SelectedItem
    {
      get;
      set;
    }

    /// <summary>
    /// Current tail data <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get;
      set;
    }

    /// <summary>
    /// Current file is valid
    /// </summary>
    public bool FileIsValid
    {
      get;
      set;
    }

    /// <summary>
    /// Splitter position
    /// </summary>
    public double SplitterPosition
    {
      get;
      set;
    }

    /// <summary>
    /// Lines read
    /// </summary>
    public int LinesRead => throw new NotFiniteNumberException();

    /// <summary>
    /// IsSmartWatch and Autorun activated
    /// </summary>
    public bool IsSmartWatchAutoRun
    {
      get;
      set;
    }

    /// <summary>
    /// SplitWindow control
    /// </summary>
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public ISplitWindowControl SplitWindow
    {
      get;
    }

    /// <summary>
    /// Open SmartWatch tail data
    /// </summary>
    /// <param name="tailData"><see cref="TailData"/></param>
    public void OpenSmartWatchTailData(TailData tailData) => throw new NotImplementedException();

    /// <summary>
    /// Create tail data window
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    public void CreateTailDataWindow(TailData item) => throw new NotImplementedException();

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public Task DisposeAsync() => throw new NotImplementedException();

    /// <summary>
    /// Stops current TailReader
    /// </summary>
    public void ExecuteStopTailCommand() => throw new NotImplementedException();

    /// <summary>
    /// Set Windows event tail reader
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    public void SetWindowsEventTailReader(TailData item) => throw new NotImplementedException();

    /// <summary>
    /// Gets parent window id
    /// </summary>
    public Guid ParentWindowId => throw new NotImplementedException();

    /// <summary>
    /// Window id
    /// </summary>
    public Guid WindowId => throw new NotImplementedException();
  }
}
