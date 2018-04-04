using System.ComponentModel;
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
  }
}
