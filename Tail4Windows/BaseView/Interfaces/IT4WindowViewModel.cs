using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.BaseView.Interfaces
{
  /// <summary>
  /// T4Window view model interface
  /// </summary>
  public interface IT4WindowViewModel : IViewModelBase
  {
    /// <summary>
    /// PreviewKeyDown command
    /// </summary>
    ICommand PreviewKeyDownCommand
    {
      get;
    }

    /// <summary>
    /// Window style
    /// </summary>
    Style T4WindowsStyle
    {
      get;
      set;
    }

    #region KeyBindings

    /// <summary>
    /// ToggleFilter command
    /// </summary>
    ICommand ToggleFilterCommand
    {
      get;
    }

    /// <summary>
    /// OpenFilterManager command
    /// </summary>
    ICommand OpenFilterManagerCommand
    {
      get;
    }

    /// <summary>
    /// OpenFileManager command
    /// </summary>
    ICommand OpenFileManagerCommand
    {
      get;
    }

    /// <summary>
    /// Call find dialog
    /// </summary>
    ICommand FindWhatCommand
    {
      get;
    }

    /// <summary>
    /// FindWhatResult command
    /// </summary>
    ICommand FindWhatResultCommand
    {
      get;
    }

    /// <summary>
    /// Go to line xxx command
    /// </summary>
    ICommand GoToLineCommand
    {
      get;
    }

    /// <summary>
    /// Quick search command
    /// </summary>
    ICommand QuickSearchCommand
    {
      get;
    }

    /// <summary>
    /// Quick save command
    /// </summary>
    ICommand QuickSaveCommand
    {
      get;
    }

    /// <summary>
    /// Toggle always on top command
    /// </summary>
    ICommand ToggleAlwaysOnTopCommand
    {
      get;
    }

    /// <summary>
    /// Open file command
    /// </summary>
    ICommand OpenFileCommand
    {
      get;
    }

    /// <summary>
    /// Clear tail log command
    /// </summary>
    ICommand ClearTailLogCommand
    {
      get;
    }

    /// <summary>
    /// Start tail command
    /// </summary>
    ICommand StartTailCommand
    {
      get;
    }

    /// <summary>
    /// Stop tail command
    /// </summary>
    ICommand StopTailCommand
    {
      get;
    }

    /// <summary>
    /// Quick add command
    /// </summary>
    ICommand QuickAddCommand
    {
      get;
    }

    /// <summary>
    /// Open font command
    /// </summary>
    ICommand OpenFontCommand
    {
      get;
    }

    /// <summary>
    /// Minimize window command
    /// </summary>
    ICommand MinimizeWindowCommand
    {
      get;
    }

    /// <summary>
    /// Open Windows event command
    /// </summary>
    ICommand OpenWindowsEventCommand
    {
      get;
    }

    /// <summary>
    /// Open help command
    /// </summary>
    ICommand OpenHelpCommand
    {
      get;
    }

    /// <summary>
    /// Show extended toolbar command
    /// </summary>
    ICommand ShowExtendedToolbarCommand
    {
      get;
    }

    /// <summary>
    /// Show bookmark overview command
    /// </summary>
    ICommand ShowBookmarkOverviewCommand
    {
      get;
    }

    #endregion

    /// <summary>
    /// Width of main window
    /// </summary>
    double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Height of main window
    /// </summary>
    double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Tray icon items source
    /// </summary>
    ObservableCollection<DragSupportMenuItem> TrayIconItemsSource
    {
      get;
      set;
    }

    /// <summary>
    /// Window X position
    /// </summary>
    double WindowPositionX
    {
      get;
      set;
    }

    /// <summary>
    /// Window Y position
    /// </summary>
    double WindowPositionY
    {
      get;
      set;
    }

    /// <summary>
    /// Window state
    /// </summary>
    WindowState WindowState
    {
      get;
      set;
    }

    /// <summary>
    /// Window closing command
    /// </summary>
    IAsyncCommand WndClosingCommand
    {
      get;
    }

    /// <summary>
    /// Tab item source
    /// </summary>
    ObservableCollection<DragSupportTabItem> TabItemsSource
    {
      get;
      set;
    }

    /// <summary>
    /// Selected <see cref="DragSupportTabItem"/>
    /// </summary>
    DragSupportTabItem SelectedTabItem
    {
      get;
      set;
    }

    /// <summary>
    /// Move some user files to new TailStore
    /// </summary>
    /// <see cref="Task"/>
    Task MoveUserFilesToTailStoreAsync();
  }
}
