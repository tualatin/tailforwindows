using System.Collections.ObjectModel;
using System.Windows.Input;
using Org.Vs.TailForWin.BaseView.ViewModels;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


namespace Org.Vs.TailForWin.BaseView.Interfaces
{
  /// <summary>
  /// Options view model interface
  /// </summary>
  public interface IOptionsViewModel : IViewModelBase
  {
    /// <summary>
    /// Close options dialog command
    /// </summary>
    ICommand CloseOptionsCommand
    {
      get;
    }

    /// <summary>
    /// Current option view model
    /// </summary>
    IOptionPage CurrentViewModel
    {
      get;
      set;
    }

    /// <summary>
    /// TreeView items
    /// </summary>
    ObservableCollection<TreeNodeOptionViewModel> Root
    {
      get;
      set;
    }

    /// <summary>
    /// Save options command
    /// </summary>
    IAsyncCommand SaveOptionsCommand
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
    /// Title
    /// </summary>
    string Title
    {
      get;
      set;
    }
  }
}
