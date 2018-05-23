using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.BaseView.Interfaces
{
  /// <summary>
  /// T4Window view model interface
  /// </summary>
  public interface IT4WindowViewModel : IViewModelBase
  {
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
  }
}
