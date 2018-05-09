using System.ComponentModel;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// LogWindow interface
  /// </summary>
  public interface ILogWindowControl : INotifyPropertyChanged
  {
    /// <summary>
    /// Lines and time changed event
    /// </summary>
    event LinesRefreshTimeChangedEventHandler OnLinesTimeChanged;

    /// <summary>
    /// On status changed event
    /// </summary>
    event StatusChangedEventHandler OnStatusChanged;

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
    /// Create tail data window
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    void CreateTailDataWindow(TailData item);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    Task DisposeAsync();
  }
}
