using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using log4net;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.Core.Native.Data.Enum;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.Keybindings;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Utils;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Interaction logic for DragWindow.xaml
  /// </summary>
  public partial class DragWindow : IDragWindow, IDragDropToTabWindow
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(DragWindow));

    private bool _hasFocus;
    private DropOverlayWindow _overlayWindow;

    #region Properties

    private DragSupportTabItem _selectedItem;

    /// <summary>
    /// Selected <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem SelectedTabItem
    {
      get => _selectedItem;
      set
      {
        _selectedItem = value;

        if ( !(_selectedItem?.Content is ILogWindowControl control) )
          return;

        EnvironmentContainer.Instance.BookmarkManager.RegisterWindowId(control.WindowId);
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new DragWindowTabItemChangedMessage(this, _selectedItem.HeaderContent, control.WindowId));
      }
    }

    #endregion


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragWindow()
    {
      InitializeComponent();

      TabItems = new ObservableCollection<DragSupportTabItem>();
      DragWindowGuid = Guid.NewGuid();

      DragWindowManager.Instance.Register(this);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenTailDataAsNewTabItem>(OnOpenTailDataAsNewTabItem);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ChangeSelectedTabItemMessage>(OnChangeSelectedTabItem);

      SourceInitialized += DragWindowSourceInitialized;

      IsParent = false;
      DataContext = this;
    }

    private void DragWindowOnClosing(object sender, CancelEventArgs e)
    {
      try
      {
        if ( !SettingsHelperController.CurrentSettings.ShouldClose )
        {
          var busyTabItems = TabItems.Where(p => p.TabItemBusyIndicator == Visibility.Visible).ToList();

          if ( busyTabItems.Count > 0 )
          {
            string message = string.Format(Application.Current.TryFindResource("ThreadIsBusy").ToString(), CoreEnvironment.ApplicationTitle);

            if ( InteractionService.ShowQuestionMessageBox(message) == MessageBoxResult.Yes )
            {
              e.Cancel = false;
            }
            else
            {
              e.Cancel = true;
              return;
            }
          }
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }

      foreach ( var tabItem in TabItems )
      {
        UiHelper.UnregisterTabItem(tabItem);
      }
    }

    private void DragWindowSourceInitialized(object sender, EventArgs e)
    {
      var handle = new WindowInteropHelper(this).Handle;
      var source = HwndSource.FromHwnd(handle);
      source?.AddHook(WndProc);
    }

    // ReSharper disable once RedundantAssignment
    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      handled = false;

      switch ( msg )
      {
      case NativeMethods.WM_ENTERSIZEMOVE:

        _hasFocus = true;
        break;

      case NativeMethods.WM_EXITSIZEMOVE:

        _hasFocus = false;
        DragWindowManager.Instance.DragEnd(this);
        break;

      case NativeMethods.WM_MOVE:

        if ( _hasFocus )
          DragWindowManager.Instance.DragMove(this);
        break;

      case NativeMethods.WM_GETMINMAXINFO:

        WindowGetMinMaxInfo.WmGetMinMaxInfo(hWnd, lParam);
        break;

      case NativeMethods.WM_WINDOWPOSCHANGING:

        WINDOWPOS pos = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

        if ( (pos.flags & (int) SWP.NOMOVE) != 0 )
          return IntPtr.Zero;

        Window wnd = (Window) HwndSource.FromHwnd(hWnd)?.RootVisual;

        if ( wnd == null )
          return IntPtr.Zero;

        bool changedPos = false;

        if ( pos.cx < MinWidth )
        {
          pos.cx = (int) MinWidth;
          changedPos = true;
        }

        if ( pos.cy < MinHeight )
        {
          pos.cy = (int) MinHeight;
          changedPos = true;
        }

        if ( !changedPos )
          return IntPtr.Zero;

        Marshal.StructureToPtr(pos, lParam, true);
        break;
      }
      return IntPtr.Zero;
    }

    /// <summary>
    /// Creates a new <see cref="DragWindow"/>
    /// </summary>
    /// <param name="left">Left</param>
    /// <param name="top">Top</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <returns><see cref="DragWindow"/></returns>
    public static DragWindow CreateTabWindow(double left, double top, double width, double height, DragSupportTabItem tabItem)
    {
      var dragWindow = new DragWindow
      {
        Left = left,
        Top = top,
        Width = width,
        Height = height,
        WindowStartupLocation = WindowStartupLocation.Manual
      };

      dragWindow.Show();
      dragWindow.Activate();
      dragWindow.Focus();

      if ( tabItem != null )
        ((IDragWindow) dragWindow).AddTabItem(tabItem);

      return dragWindow;
    }

    /// <summary>
    /// TabItem source
    /// </summary>
    public ObservableCollection<DragSupportTabItem> TabItems
    {
      get;
      set;
    }

    /// <summary>
    /// Drag window <see cref="Guid"/>
    /// </summary>
    public Guid DragWindowGuid
    {
      get;
    }

    /// <summary>
    /// Add TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public void AddTabItem(DragSupportTabItem tabItem) => AddTabItem(tabItem.HeaderContent, tabItem.HeaderToolTip, tabItem.TabItemBusyIndicator, tabItem.TabItemBackgroundColorStringHex, (LogWindowControl) tabItem.Content);

    /// <summary>
    /// Remove TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <param name="dragWndRemove">Drag window remove</param>
    public void RemoveTabItem(DragSupportTabItem tabItem, bool dragWndRemove)
    {
      if ( !TabItems.Contains(tabItem) )
        return;

      if ( !dragWndRemove && tabItem.TabItemBusyIndicator == Visibility.Visible )
      {
        string message = $"{Application.Current.TryFindResource("QRemoveTab")} \n {tabItem.HeaderFullText}";

        if ( InteractionService.ShowQuestionMessageBox(message) == MessageBoxResult.No )
          return;
      }

      tabItem.TabHeaderDoubleClick -= TabItemTabHeaderDoubleClick;
      tabItem.CloseTabWindow -= TabItemCloseTabWindow;

      UiHelper.UnregisterTabItem(tabItem);
      TabItems.Remove(tabItem);

      if ( TabItems.Count == 0 )
        Close();
    }

    /// <summary>
    /// Is parent window
    /// </summary>
    public bool IsParent
    {
      get;
    }

    /// <summary>
    /// On Drag enter
    /// </summary>
    public void OnDragEnter()
    {
      if ( _overlayWindow == null )
        _overlayWindow = new DropOverlayWindow();

      if ( WindowState == WindowState.Maximized )
      {
        _overlayWindow.Left = 0;
        _overlayWindow.Top = 0;
      }
      else
      {
        _overlayWindow.Left = Left;
        _overlayWindow.Top = Top;
      }
      _overlayWindow.Width = ActualWidth;
      _overlayWindow.Height = ActualHeight;
      _overlayWindow.Topmost = true;

      _overlayWindow.Show();
    }

    /// <summary>
    /// On Drag leave
    /// </summary>
    public void OnDragLeave()
    {
      if ( _overlayWindow == null )
        return;

      _overlayWindow.Close();
      _overlayWindow = null;
    }

    /// <summary>
    /// Is drag mouse ober
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over<c>True</c> otherwise <c>False</c></returns>
    public bool IsDragMouseOver(Point mousePosition)
    {
      if ( WindowState == WindowState.Minimized )
        return false;

      double left, top;

      if ( WindowState == WindowState.Maximized )
      {
        left = 0;
        top = 0;
      }
      else
      {
        left = Left;
        top = Top;
      }

      bool isMouseOver = mousePosition.X > left && mousePosition.X < left + ActualWidth && mousePosition.Y > top && mousePosition.Y < top + ActualHeight;

      return isMouseOver;
    }

    /// <summary>
    /// Is drag mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over <c>True</c> otherwise <c>False</c></returns>
    public bool IsDragMouseOverTabZone(Point mousePosition) => _overlayWindow?.IsMouseOverTabTarget(mousePosition) ?? false;

    #region Events

    private void TabItemTabHeaderDoubleClick(object sender, RoutedEventArgs e)
    {
      LOG.Trace("MouseDoubleClick");
    }

    private void TabItemCloseTabWindow(object sender, RoutedEventArgs e)
    {
      if ( e.Source is DragSupportTabItem item )
        RemoveTabItem(item, false);
    }

    private void TabControlOnAddTabItemEvent(object sender, RoutedEventArgs e) => AddTabItem($"{Application.Current.TryFindResource("NoFile")}", $"{Application.Current.TryFindResource("NoFile")}", Visibility.Collapsed);

    #endregion

    private void AddTabItem(string header, string toolTip, Visibility busyIndicator, string backgroundColor = "#FFD6DBE9", ILogWindowControl content = null)
    {
      var tabItem = UiHelper.CreateDragSupportTabItem(header, toolTip, busyIndicator, content, backgroundColor);

      tabItem.CloseTabWindow += TabItemCloseTabWindow;
      tabItem.TabHeaderDoubleClick += TabItemTabHeaderDoubleClick;

      TabItems.Add(tabItem);

      if ( content == null )
        return;
      if ( !content.CurrentTailData.FilterState )
        return;

      // Fuck off WPF databinding, set filter state false and than true again -> Highlighting works.
      content.CurrentTailData.FilterState = false;
      content.CurrentTailData.FilterState = true;

      // Commit changes
      content.CurrentTailData.CommitChanges();
    }

    #region Commands

    #endregion

    #region Command functions

    #endregion

    #region KeyBinding commands

    private ICommand _goToLineCommand;

    /// <summary>
    /// Go to line xxx command
    /// </summary>
    public ICommand GoToLineCommand => _goToLineCommand ?? (_goToLineCommand = new RelayCommand(p => ExecuteGoToLineCommand()));

    private ICommand _addNewTabItemCommand;

    /// <summary>
    /// Add new tab item command
    /// </summary>
    public ICommand AddNewTabItemCommand => _addNewTabItemCommand ?? (_addNewTabItemCommand = new RelayCommand(p => ExecuteAddNewTabItemCommand()));

    private ICommand _closeTabItemCommand;

    /// <summary>
    /// Add new tab item command
    /// </summary>
    public ICommand CloseTabItemCommand => _closeTabItemCommand ?? (_closeTabItemCommand = new RelayCommand(p => ExecuteCloseTabItemCommand()));

    private ICommand _findWhatCommand;

    /// <summary>
    /// FindWhat command
    /// </summary>
    public ICommand FindWhatCommand => _findWhatCommand ?? (_findWhatCommand = new RelayCommand(p => ExecuteFindWhatCommand()));

    private ICommand _findWhatResultCommand;

    /// <summary>
    /// FindWhatResult command
    /// </summary>
    public ICommand FindWhatResultCommand => _findWhatResultCommand ?? (_findWhatResultCommand = new RelayCommand(p => ExecuteFindWhatResultCommand()));

    private ICommand _toggleAlwaysOnTopCommand;

    /// <summary>
    /// Toggle always on top command
    /// </summary>
    public ICommand ToggleAlwaysOnTopCommand => _toggleAlwaysOnTopCommand ?? (_toggleAlwaysOnTopCommand = new RelayCommand(p => ExecuteToggleAlwaysOnTopCommand()));

    private ICommand _openFileManagerCommand;

    /// <summary>
    /// OpenFileManager command
    /// </summary>
    public ICommand OpenFileManagerCommand => _openFileManagerCommand ?? (_openFileManagerCommand = new RelayCommand(p => ExecuteOpenFileManagerCommand()));

    private ICommand _openFilterManagerCommand;

    /// <summary>
    /// OpenFilterManager command
    /// </summary>
    public ICommand OpenFilterManagerCommand => _openFilterManagerCommand ?? (_openFilterManagerCommand = new RelayCommand(p => ExecuteOpenFilterManagerCommand()));

    private ICommand _toggleFilterCommand;

    /// <summary>
    /// ToggleFilter command
    /// </summary>
    public ICommand ToggleFilterCommand => _toggleFilterCommand ?? (_toggleFilterCommand = new RelayCommand(p => ExecuteToggleFilterCommand()));

    private ICommand _quickSaveCommand;

    /// <summary>
    /// Quick save command
    /// </summary>
    public ICommand QuickSaveCommand => _quickSaveCommand ?? (_quickSaveCommand = new RelayCommand(p => ExecuteQuickSaveCommand()));

    private ICommand _openFileCommand;

    /// <summary>
    /// Open file command
    /// </summary>
    public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(p => ExecuteOpenFileCommand()));

    private ICommand _clearTailLogCommand;

    /// <summary>
    /// Clear tail log command
    /// </summary>
    public ICommand ClearTailLogCommand => _clearTailLogCommand ?? (_clearTailLogCommand = new RelayCommand(p => ExecuteClearTailLogCommand()));

    private ICommand _startTailCommand;

    /// <summary>
    /// Start tail command
    /// </summary>
    public ICommand StartTailCommand => _startTailCommand ?? (_startTailCommand = new RelayCommand(p => ExecuteStartTailCommand()));

    private ICommand _stopTailCommand;

    /// <summary>
    /// Stop tail command
    /// </summary>
    public ICommand StopTailCommand => _stopTailCommand ?? (_stopTailCommand = new RelayCommand(p => ExecuteStopTailCommand()));

    private ICommand _quickAddCommand;

    /// <summary>
    /// Quick add command
    /// </summary>
    public ICommand QuickAddCommand => _quickAddCommand ?? (_quickAddCommand = new RelayCommand(p => ExecuteQuickAddCommand()));

    private ICommand _openFontCommand;

    /// <summary>
    /// Open font command
    /// </summary>
    public ICommand OpenFontCommand => _openFontCommand ?? (_openFontCommand = new RelayCommand(p => ExecuteOpenFontCommand()));

    private ICommand _minimizeWindowCommand;

    /// <summary>
    /// Minimize window command
    /// </summary>
    public ICommand MinimizeWindowCommand => _minimizeWindowCommand ?? (_minimizeWindowCommand = new RelayCommand(p => ExecuteMinimizeWindowCommand((Window) p)));

    private ICommand _openWindowsEventCommand;

    /// <summary>
    /// Open Windows event command
    /// </summary>
    public ICommand OpenWindowsEventCommand => _openWindowsEventCommand ?? (_openWindowsEventCommand = new RelayCommand(p => ExecuteOpenWindowsEventCommand()));

    private ICommand _openHelpCommand;

    /// <summary>
    /// Open help command
    /// </summary>
    public ICommand OpenHelpCommand => _openHelpCommand ?? (_openHelpCommand = new RelayCommand(p => ExecuteOpenHelpCommand()));

    private ICommand _showExtendedToolbarCommand;

    /// <summary>
    /// Show extended toolbar command
    /// </summary>
    public ICommand ShowExtendedToolbarCommand => _showExtendedToolbarCommand ?? (_showExtendedToolbarCommand = new RelayCommand(p => ExecuteShowExtendedToolbarCommand()));

    private ICommand _showBookmarkOverviewCommand;

    /// <summary>
    /// Show bookmark overview command
    /// </summary>
    public ICommand ShowBookmarkOverviewCommand => _showBookmarkOverviewCommand ?? (_showBookmarkOverviewCommand = new RelayCommand(p => ExecuteShowBookmarkOverviewCommand()));

    #endregion

    #region KeyBinding command functions

    private void ExecuteShowBookmarkOverviewCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowBookmarkOverviewMessage(control.WindowId));
    }

    private void ExecuteShowExtendedToolbarCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowExtendedToolbarMessage(control.WindowId));
    }

    private void ExecuteOpenHelpCommand()
    {
      var url = new Uri(EnvironmentContainer.ApplicationHelpUrl);
      EnvironmentContainer.Instance.ExecuteRequestNavigateCommand(url);
    }

    private void ExecuteOpenWindowsEventCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenWindowsEventMessage(control.WindowId));
    }

    private void ExecuteMinimizeWindowCommand(Window window) => window.WindowState = WindowState.Minimized;

    private void ExecuteOpenFontCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFontSettingsMessage(control.WindowId));
    }

    private void ExecuteQuickAddCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickAddMessage(control.WindowId));
    }

    private void ExecuteStopTailCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StopTailMessage(control.WindowId));
    }

    private void ExecuteStartTailCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StartTailMessage(control.WindowId));
    }

    private void ExecuteClearTailLogCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ClearTailLogMessage(control.WindowId));
    }

    private void ExecuteOpenFileCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFileMessage(control.WindowId));
    }

    private void ExecuteQuickSaveCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickSaveMessage(control.WindowId));
    }

    private void ExecuteToggleFilterCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ToggleFilterMessage(control.WindowId));
    }

    private void ExecuteOpenFilterManagerCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFilterManagerMessage(control.WindowId));
    }

    private void ExecuteOpenFileManagerCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailManagerMessage(control.WindowId));
    }

    private void ExecuteFindWhatResultCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFindWhatResultWindowMessage(control.SplitWindow.FindWhatResults, control.WindowId));
    }

    private void ExecuteToggleAlwaysOnTopCommand() => SettingsHelperController.CurrentSettings.AlwaysOnTop = !SettingsHelperController.CurrentSettings.AlwaysOnTop;

    private void ExecuteFindWhatCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      string findWhat = control.SplitWindow.SelectedText;
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFindWhatWindowMessage(this, SelectedTabItem.HeaderContent, control.WindowId, findWhat));
    }

    private void ExecuteGoToLineCommand()
    {
      if ( SelectedTabItem == null )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenGoToLineDialogMessage(((ILogWindowControl) SelectedTabItem.Content).ParentWindowId));
    }

    private void ExecuteAddNewTabItemCommand() => AddTabItem($"{Application.Current.TryFindResource("NoFile")}", $"{Application.Current.TryFindResource("NoFile")}", Visibility.Collapsed);

    private void ExecuteCloseTabItemCommand()
    {
      if ( SelectedTabItem == null )
        return;

      RemoveTabItem(SelectedTabItem, false);
    }

    #endregion

    private void OnChangeSelectedTabItem(ChangeSelectedTabItemMessage args)
    {
      DragSupportTabItem result = UiHelper.GetTabItemList().FirstOrDefault(p => ((ILogWindowControl) p.Content).WindowId == args.WindowId);

      if ( result == null )
        return;

      result.IsSelected = true;
      ((ILogWindowControl) result.Content).OpenSmartWatchTailData(args.TailData);
    }

    private void OnOpenTailDataAsNewTabItem(OpenTailDataAsNewTabItem args)
    {
      if ( args.ParentGuid != DragWindowGuid )
        return;

      // Workaround unregister OpenTailDataAsNewTabItem to prevent open this message more than one times. Very, very ugly!
      // Register this message again, after waiting some ms...
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenTailDataAsNewTabItem>(OnOpenTailDataAsNewTabItem);

      ILogWindowControl content = new LogWindowControl
      {
        LogWindowTabItem = new DragSupportTabItem(),
        CurrentTailData = args.TailData,
        SelectedItem = args.TailData.FileName,
        IsSmartWatchAutoRun = args.IsSmartWatch && args.TailData.AutoRun
      };
      AddTabItem(args.TailData.File, args.TailData.FileName, Visibility.Collapsed, content: content);

      new ThrottledExecution().InMs(100).Do(() =>
      {
        EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenTailDataAsNewTabItem>(OnOpenTailDataAsNewTabItem);
      });
    }

    private void DragWindowOnActivated(object sender, EventArgs e)
    {
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new SetFloatingTopmostFlagMessage(true));

      if ( SelectedTabItem == null || !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.BookmarkManager.RegisterWindowId(control.WindowId, true);
    }

    private void DragWindowOnDeactivated(object sender, EventArgs e) => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new SetFloatingTopmostFlagMessage(false));
  }
}
