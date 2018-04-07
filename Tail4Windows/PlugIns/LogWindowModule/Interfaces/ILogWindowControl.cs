using System.ComponentModel;
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
    /// Current tail data <see cref="TailData"/>
    /// </summary>
    TailData CurrenTailData
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
  }
}
