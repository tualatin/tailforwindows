using System.ComponentModel;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// LogWindow interface
  /// </summary>
  public interface ILogWindow : INotifyPropertyChanged
  {
    /// <summary>
    /// LogWindowTabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    DragSupportTabItem LogWindowTabItem
    {
      get;
    }

    /// <summary>
    /// Current LogWindowState <see cref="EStatusbarState"/>
    /// </summary>
    EStatusbarState LogWindowState
    {
      get;
    }

    /// <summary>
    /// Is selected
    /// </summary>
    bool IsSelected
    {
      get;
    }

    /// <summary>
    /// Current tail data <see cref="TailData"/>
    /// </summary>
    TailData CurrenTailData
    {
      get;
    }

    /// <summary>
    /// Current file is valid
    /// </summary>
    bool FileIsValid
    {
      get;
    }
  }
}
