using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.BaseView.UserControls.Interfaces
{
  /// <summary>
  /// T4Window view model interface
  /// </summary>
  public interface IT4WindowViewModel : IViewModelBase
  {
    /// <summary>
    /// CurrentBusy state
    /// </summary>
    string CurrentBusyState
    {
      get;
      set;
    }

    /// <summary>
    /// CurrentStatusBarBackground color as string
    /// </summary>
    string CurrentStatusBarBackgroundColorHex
    {
      get;
      set;
    }

    /// <summary>
    /// Go to line xxx command
    /// </summary>
    ICommand GoToLineCommand
    {
      get;
    }

    /// <summary>
    /// PreviewKeyDown command
    /// </summary>
    ICommand PreviewKeyDownCommand
    {
      get;
    }

    /// <summary>
    /// PreviewTrayContextMenuOpen command
    /// </summary>
    ICommand PreviewTrayContextMenuOpenCommand
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
    /// Window style
    /// </summary>
    Style T4WindowsStyle
    {
      get;
      set;
    }

    /// <summary>
    /// Toggle always on top command
    /// </summary>
    ICommand ToggleAlwaysOnTopCommand
    {
      get;
    }

    /// <summary>
    /// TrayContextMenuOpen command
    /// </summary>
    ICommand TrayContextMenuOpenCommand
    {
      get;
    }

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
    ObservableCollection<MenuItem> TrayIconItemsSource
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
  }
}
