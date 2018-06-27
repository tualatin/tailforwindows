using System.Collections.ObjectModel;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.PlugIns.WindowEventReadModule.ViewModels;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.WindowEventReadModule.Interfaces
{
  /// <summary>
  /// Windows event categories view model interface
  /// </summary>
  public interface IWindowsEventCategoriesViewModel : IViewModelBase
  {
    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    TailData CurrentTailData
    {
      get;
    }

    /// <summary>
    /// TreeView items
    /// </summary>
    ObservableCollection<TreeNodeWindowsEventsViewModel> Root
    {
      get;
    }

    /// <summary>
    /// Set selected item command
    /// </summary>
    ICommand SetSelectedItemCommand
    {
      get;
    }

    /// <summary>
    /// Open command
    /// </summary>
    ICommand OpenCommand
    {
      get;
    }
  }
}
