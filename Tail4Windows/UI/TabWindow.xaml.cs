using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Data.Events;
using Org.Vs.TailForWin.Data.Native;
using Org.Vs.TailForWin.Data.Native.Enum;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Native;
using Org.Vs.TailForWin.Template;
using Org.Vs.TailForWin.UI.Utils;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.UI
{
  /// <summary>
  /// Tab window logic
  /// </summary>
  public partial class TabWindow : Window, ITabWindow, IDragDropToTabWindow
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TabWindow));

    private OverlayDragWnd overlayWindow;
    private bool hasFocus;
    private TailForWinTabItem tabAdd;
    private SearchDialog searchBoxWindow;
    private TailLog currentPage;
    private string parameterFileName;
    private bool ctrlTabKey;
    private bool shiftCtrlTabKey;

    #region Properties

    /// <summary>
    /// Set ToolTip detail text
    /// </summary>
    public string ToolTipDetailText
    {
      get => fancyToolTipTfW.ToolTipDetail;
      set => fancyToolTipTfW.ToolTipDetail = value;
    }

    /// <summary>
    /// Main window taskbar icon
    /// </summary>
    public NotifyIcon.TaskbarIcon MainWndTaskBarIcon => tbIcon;

    /// <summary>
    /// Uptime start time
    /// </summary>
    public DateTime TfWUpTimeStart
    {
      get;
      private set;
    }

    /// <summary>
    /// Set topmost
    /// </summary>
    public bool MainWindowTopmost
    {
      get => Topmost;
      set => Topmost = value;
    }

    /// <summary>
    /// Set statusbar state item
    /// </summary>
    public StatusBarItem StatusBarState => stsBarState;

    /// <summary>
    /// Set statusbar encoding item
    /// </summary>
    public StatusBarItem StatusBarEncoding => stsEncoding;

    /// <summary>
    /// Set statusbar lines read
    /// </summary>
    public StatusBarItem StatusBarLinesRead => stsLinesRead;

    /// <summary>
    /// Set statubar encode combobox (cbStsEncoding)
    /// </summary>
    public ComboBox StatusBarEncodeCb
    {
      get => cbStsEncoding;
    }

    /// <summary>
    /// Current tab control
    /// </summary>
    public DragSupportTabControl TabControl => tabControl;

    #endregion


    /// <summary>
    /// Standarc constructor
    /// </summary>
    public TabWindow()
    {
      InitializeComponent();
      DefaultWndSettings();

      tabControl.SelectionChanged += TabControl_SelectionChanged;
      tabControl.PreviewKeyDown += TabControl_PreviewKeyDown;
      tabControl.PreviewKeyUp += TabControl_PreviewKeyUp;
      tabControl.Drop += TabControl_Drop;

      tabAdd = new TailForWinTabItem
      {
        Header = "+",
        Name = "AddChildTab",
        Style = (Style) FindResource("TabItemAddStyle")
      };
      tabAdd.PreviewMouseLeftButtonDown += TabAdd_MouseLeftButtonDown;

      tabControl.Items.Add(tabAdd);
      AddTabItem();
      DragWindowManager.Instance.Register(this);

      SourceInitialized += TabWindow_SourceInitialized;
      PreviewMouseDown += TabWindow_PreviewMouseDown;
      Loaded += TabWindow_Loaded;
      Closing += TabWindow_Closing;
    }

    /// <summary>
    /// Releases all resources used by the MainWindow.
    /// </summary>
    public void Dispose()
    {
      try
      {
        foreach(TabItem item in tabControl.Items)
        {
          if(item.Content != null && item.Content.GetType() == typeof(Frame))
          {
            var page = GetTailLogWindow(item.Content as Frame);

            if(page == null)
              continue;

            page.StopThread();
          }
        }
      }
      finally
      {
        if(currentPage != null)
        {
          currentPage.Dispose();
          currentPage = null;
        }
      }
    }

    /// <summary>
    /// Set statusbar text
    /// </summary>
    public void SetSbIconText()
    {
      if(currentPage == null)
        return;

      try
      {
        if(currentPage.IsThreadBusy)
          MainWndTaskBarIcon.ToolTipText = Application.Current.FindResource("Record") as string;
        else
          MainWndTaskBarIcon.ToolTipText = Application.Current.FindResource("TrayIconReady") as string;
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Opens file from parameter
    /// </summary>
    /// <param name="fName">Name of file</param>
    public void OpenFileFromParameter(string fName)
    {
      parameterFileName = fName;
    }

    /// <summary>
    /// OnOpenFileManager tab
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void FileManagerTab(object sender, EventArgs e)
    {
      if(e is FileManagerDataEventArgs properties)
      {
        if(properties == null)
          return;

        FileManagerData item = properties.GetData();

        if(item == null)
          return;

        AddTabItem(item);
        
        // TODO issue! Improve it...
        if(tabControl.Items.Count == 3)
        {
          var tabItem = tabControl.Items[0];
          RemoveTabItem(tabItem as TabItem);
        }
      }
    }

    /// <summary>
    /// Create a new tab window
    /// </summary>
    /// <param name="left">Left</param>
    /// <param name="top">Top</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="tabItem">TabItem</param>
    /// <returns>New instance of TabWindow</returns>
    public static TabWindow CreateTabWindow(double left, double top, double width, double height, TabItem tabItem)
    {
      TabWindow tabWin = new TabWindow
      {
        Width = width,
        Height = height,
        Left = left,
        Top = top,
        WindowStartupLocation = WindowStartupLocation.Manual,
      };
      Control tabContent = tabItem.Content as Control;

      if(tabContent == null)
      {
        tabContent = new ContentControl();
        ((ContentControl) tabContent).Content = tabItem.Content;
      }

      ((ITabWindow) tabWin).AddTabItem(tabItem.Header.ToString(), tabContent);
      tabWin.Show();
      tabWin.Activate();
      tabWin.Focus();

      return (tabWin);
    }

    #region Events

    private void TabAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(sender is TailForWinTabItem)
      {
        var tabItem = sender as TailForWinTabItem;

        if(tabItem.Equals(tabAdd))
          AddTabItem();
      }
    }

    private void TabControl_Drop(object sender, DragEventArgs e)
    {
      // TODO issue?? Drag and drop on TabControl
      currentPage?.DropHelper(sender, e);
    }

    private void TabControl_PreviewKeyUp(object sender, KeyEventArgs e)
    {
      ctrlTabKey = false;
      shiftCtrlTabKey = false;
    }

    private void TabControl_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.Tab)
        ctrlTabKey = true;
      if((Keyboard.Modifiers & ModifierKeys.Control) != 0 && (Keyboard.Modifiers & ModifierKeys.Shift) != 0 && e.Key == Key.Tab)
        shiftCtrlTabKey = true;
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // TODO issue, when all files opens from FileManager, it works. But when open a file from external, it doesn't work again, it's broken!!
      if(!IsInitialized)
        return;

      if(e.Source is TabControl tabControl)
      {
        if(e.AddedItems.Count == 0)
          return;

        LOG.Trace("{0}", System.Reflection.MethodBase.GetCurrentMethod());

        var tab = e.AddedItems[0] as TabItem;

        if(tab == null || tabControl == null)
          return;

        e.Handled = true;

        if(tab.Equals(tabAdd) && !ctrlTabKey && !shiftCtrlTabKey)
        {
          if(tabControl.Items.Count >= 2)
            tabControl.SelectedItem = tabControl.Items[tabControl.Items.Count - 2];

          return;
        }

        if(tab.Equals(tabAdd) && ctrlTabKey && !shiftCtrlTabKey)
        {
          // Scroll with CTRL + TAB, start from beginning
          tabControl.SelectedItem = tabControl.Items[0];
        }
        else if(tab.Equals(tabAdd) && ctrlTabKey && shiftCtrlTabKey)
        {
          // Scroll with CTRL + SHIFT + TAB, start from end
          tabControl.SelectedItem = tabControl.Items[tabControl.Items.Count - 2];
        }

        TailLog page = GetTailLogWindow(tab.Content as Frame);

        if(page == null)
          return;

        if(currentPage != null && (page.GetChildTabIndex() == currentPage.GetChildTabIndex()))
          return;

        stsBarState.Content = page.GetChildState();
        page.ActiveTab = true;

        if(searchBoxWindow.Visibility == Visibility.Visible)
        {
          page.SearchBoxActive();
          page.WrapAround(searchBoxWindow.WrapSearch);
          FindWhatTextChangedEvent(this, EventArgs.Empty);
          searchBoxWindow.SetTitle = string.IsNullOrEmpty(page.FileManagerProperties.File) ? page.TailLogProperties.File : page.FileManagerProperties.File;
        }

        currentPage = page;
        TabItemUpdateParent(page);

        SetSbIconText();
      }
    }

    private void TabWindow_Loaded(object sender, RoutedEventArgs e)
    {
      // Important for command line parameter!
      if(LogFile.APP_MAIN_WINDOW == null)
        LogFile.APP_MAIN_WINDOW = (TabWindow) DragWindowManager.Instance.AllWindows.Last();// (Application.Current.MainWindow as TabWindow);

      if(SettingsHelper.TailSettings.AutoUpdate)
        AutoUpdate.Init();

      TfWUpTimeStart = DateTime.Now;
      LOG.Info("Startup completed!");
    }

    private void TabWindow_Closing(object sender, CancelEventArgs e)
    {
      bool tailing = false;

      foreach(TabItem item in tabControl.Items)
      {
        if(item.Content != null && item.Content.GetType() == typeof(Frame))
        {
          var page = GetTailLogWindow(item.Content as Frame);

          if(page == null)
            continue;

          if(page.IsThreadBusy)
          {
            tailing = true;
            break;
          }
        }
      }

      if(tailing)
      {
        string message = string.Format(Application.Current.FindResource("ThreadIsBusy").ToString(), LogFile.APPLICATION_CAPTION);

        if(MessageBox.Show(message, LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
          e.Cancel = false;
        }
        else
        {
          e.Cancel = true;
          return;
        }
      }

      LOG.Trace("{0} closing, goodbye!", LogFile.APPLICATION_CAPTION);
      OnExit();
    }

    private void TabWindow_SourceInitialized(object sender, EventArgs e)
    {
      HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
      source.AddHook(new HwndSourceHook(WndProc));
    }

    private void TabWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if(e.MiddleButton != MouseButtonState.Pressed)
        return;

      Point mousePoint = PointToScreen(Mouse.GetPosition(this));
      bool addNew = false;

      foreach(TabItem item in tabControl.Items)
      {
        Point relativePoint = item.PointToScreen(new Point(0, 0));
        System.Drawing.Rectangle rc = new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) item.DesiredSize.Width, (int) item.DesiredSize.Height);

        if(rc.Contains((int) mousePoint.X, (int) mousePoint.Y))
          return;

        addNew = true;
      }

      if(!addNew)
        return;

      AddTabItem();
    }

    private void TabItem_TabHeaderDoubleClick(object sender, RoutedEventArgs e)
    {
      // When one tab and the add tab is open, no other operation possible
      if(tabControl.Items.Count <= 2)
        return;

      if(e.Source is TailForWinTabItem tabItem)
      {
        Point mousePos = PointToScreen(Mouse.GetPosition(tabItem));
        TabWindow tabWin = CreateTabWindow(mousePos.X, mousePos.Y, ActualWidth, ActualHeight, tabItem);

        tabControl.RemoveTabItem(tabItem);
        tabWin.Activate();
        tabWin.Focus();
      }
    }

    private void TabItem_CloseTabWindow(object sender, RoutedEventArgs e)
    {
      if(e.Source is TailForWinTabItem tabItem)
        RemoveTabItem(tabItem);
    }

    private void OnContentRendered(object sender, EventArgs e)
    {
      if(sender is Frame)
      {
        Frame frame = sender as Frame;

        if(frame == null || frame.Content == null)
          return;

        TailLog page = frame.Content as TailLog;

        if(page == null)
          return;

        currentPage = page;
        TabItemUpdateParent(page);

        if(!string.IsNullOrEmpty(parameterFileName))
        {
          currentPage.OpenFileFromParameter(parameterFileName);

          // after opens file name from parameter, reset parameterFileName
          parameterFileName = string.Empty;
        }
      }
    }

    private void CbStsEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      currentPage?.UpdateFileEncoding((Encoding) cbStsEncoding.SelectedItem);
    }

    private static void DoubleClickNotifyIcon(object sender, EventArgs e)
    {
      LogFile.BringMainWindowToFront();
    }

    #endregion

    #region ITabWindow

    /// <summary>
    /// Tab items collection
    /// </summary>
    public ItemCollection TabItems
    {
      get => tabControl.Items;
    }

    /// <summary>
    /// Tab header selected
    /// </summary>
    public string TabHeaderSelected
    {
      get
      {
        if(tabControl.SelectedItem is TailForWinTabItem tab)
          return (tab.Header.ToString());

        return (string.Empty);
      }
      set
      {
        SelectTabItem(value);
      }
    }

    /// <summary>
    /// Add tab item to control when drag and drop window
    /// </summary>
    /// <param name="tabHeader">Tab header</param>
    /// <param name="content">Content</param>
    public void AddTabItem(string tabHeader, Control content)
    {
      AddTabItem(tabHeader, content, null);
    }

    /// <summary>
    /// Add tab item to control
    /// </summary>
    /// <param name="properties">File Manager data object, <c>default</c> is <c>NULL</c></param>
    public void AddTabItem(FileManagerData properties = null)
    {
      AddTabItem(string.Empty, null, properties);
    }

    private void AddTabItem(string tabHeader, Control content, FileManagerData properties)
    {
      if(tabControl.Items.Count >= LogFile.MAX_TAB_CHILDS)
      {
        MessageBox.Show(Application.Current.FindResource("HCloseTab") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      TailForWinTabItem item = new TailForWinTabItem
      {
        Header = string.IsNullOrEmpty(tabHeader) ? LogFile.TABBAR_CHILD_EMPTY_STRING : tabHeader,
        Name = $"TabItem_{tabControl.Items.Count}",
        Style = (Style) FindResource("TabItemStopStyle")
      };
      item.TabHeaderDoubleClick += TabItem_TabHeaderDoubleClick;
      item.CloseTabWindow += TabItem_CloseTabWindow;

      if(content != null)
        item.Content = content;
      else
        item.Content = CreateTabItemContent(item, properties);

      tabControl.Items.Insert(tabControl.Items.Count - 1, item);
      tabControl.SelectedItem = item;
    }

    /// <summary>
    /// Remove a tab item
    /// </summary>
    /// <param name="tabItem">Item to remove</param>
    public void RemoveTabItem(TabItem tabItem)
    {
      if(tabControl.Items.Contains(tabItem))
      {
        TailLog page = GetTailLogWindow(tabItem.Content as Frame);

        try
        {
          if(page != null)
          {
            if(page.IsThreadBusy)
            {
              if(MessageBox.Show(string.Format("{0} '{1}'?", Application.Current.FindResource("QRemoveTab"), tabItem.Header),
                          LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            }

            FileManagerHelper item = LogFile.FmHelper.SingleOrDefault(x => x.ID == page.FileManagerProperties.ID);

            if(item != null)
              LogFile.FmHelper.Remove(item);

            page.StopThread();
          }
        }
        catch(ArgumentNullException ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }

        ((TailForWinTabItem) tabItem).TabHeaderDoubleClick -= TabItem_TabHeaderDoubleClick;
        ((TailForWinTabItem) tabItem).CloseTabWindow -= TabItem_CloseTabWindow;
        tabControl.Items.Remove(tabItem);

        if(tabControl.Items.Count < 2)
          AddTabItem();
      }
    }

    #endregion

    #region IDragDropToTabWindow

    /// <summary>
    /// Add TabItem
    /// </summary>
    public TabItem TabAdd => tabAdd;

    /// <summary>
    /// Is drag mouse over
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    public bool IsDragMouseOver(Point mousePosition)
    {
      if(WindowState == WindowState.Minimized)
        return (false);

      double left, top;

      if(WindowState == WindowState.Maximized)
      {
        left = 0;
        top = 0;
      }
      else
      {
        left = Left;
        top = Top;
      }

      bool isMouseOver = (mousePosition.X > left && mousePosition.X < (left + ActualWidth) && mousePosition.Y > top && mousePosition.Y < (top + ActualHeight));

      return (isMouseOver);
    }

    /// <summary>
    /// Is mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    public bool IsDragMouseOverTabZone(Point mousePosition)
    {
      if(overlayWindow == null)
        return (false);

      return (overlayWindow.IsMouseOverTabTarget(mousePosition));
    }

    /// <summary>
    /// Drag leave
    /// </summary>
    public void OnDrageLeave()
    {
      if(overlayWindow != null)
      {
        overlayWindow.Close();
        overlayWindow = null;
      }
    }

    /// <summary>
    /// Drag enter
    /// </summary>
    public void OnDragEnter()
    {
      if(overlayWindow == null)
        overlayWindow = new OverlayDragWnd();

      if(WindowState == WindowState.Maximized)
      {
        overlayWindow.Left = 0;
        overlayWindow.Top = 0;
      }
      else
      {
        overlayWindow.Left = Left;
        overlayWindow.Top = Top;
      }

      overlayWindow.Width = ActualWidth;
      overlayWindow.Height = ActualHeight;
      overlayWindow.Topmost = true;

      overlayWindow.Show();
    }

    #endregion

    #region HelperFunctions

    private void DefaultWndSettings()
    {
      SettingsHelper.ReadSettings();
      LogFile.InitObservableCollectionsRrtpfe();

      switch(SettingsHelper.TailSettings.CurrentWindowStyle)
      {
      case EWindowStyle.ModernBlueWindowStyle:

        MainWindow.Style = (Style) FindResource("Tail4WindowStyle");
        break;
      }

      tbIcon.ToolTipText = Application.Current.FindResource("TrayIconReady") as string;
      fancyToolTipTfW.ApplicationText = LogFile.APPLICATION_CAPTION;
      tbIcon.TrayMouseDoubleClick += DoubleClickNotifyIcon;

      Title = LogFile.APPLICATION_CAPTION;
      Topmost = SettingsHelper.TailSettings.AlwaysOnTop;

      cbStsEncoding.DataContext = LogFile.FileEncoding;
      cbStsEncoding.DisplayMemberPath = "HeaderName";

      PreviewKeyDown += HandleMainWindowKeys;
      searchBoxWindow = new SearchDialog();

      MoveIntoView();
      RestoreWindowSizePosition();

      // EventHandler for searchBoxWindow
      searchBoxWindow.FindNextEvent += FindNextEvent;
      searchBoxWindow.CountSearchEvent += CoundSearchEvent;
      searchBoxWindow.HideSearchBox += HideSearchBoxEvent;
      searchBoxWindow.FindTextChanged += FindWhatTextChangedEvent;
      searchBoxWindow.WrapAround += WrapAroundEvent;
      searchBoxWindow.BookmarkLine += BookmarkLineEvent;
    }

    private void RestoreWindowSizePosition()
    {
      if(SettingsHelper.TailSettings.CurrentWindowState == WindowState.Normal)
      {
        if(SettingsHelper.TailSettings.RestoreWindowSize)
        {
          if(SettingsHelper.TailSettings.WndWidth != -1.0f)
            Application.Current.MainWindow.Width = SettingsHelper.TailSettings.WndWidth;
          if(SettingsHelper.TailSettings.WndHeight != -1.0f)
            Application.Current.MainWindow.Height = SettingsHelper.TailSettings.WndHeight;
        }

        if(SettingsHelper.TailSettings.SaveWindowPosition)
        {
          if(SettingsHelper.TailSettings.WndYPos != -1.0f)
            Application.Current.MainWindow.Top = SettingsHelper.TailSettings.WndYPos;
          if(SettingsHelper.TailSettings.WndXPos != -1.0f)
            Application.Current.MainWindow.Left = SettingsHelper.TailSettings.WndXPos;
        }
      }
      else
      {
        MainWindow.WindowState = SettingsHelper.TailSettings.CurrentWindowState;
      }
    }

    private void MoveIntoView()
    {
      if(SettingsHelper.TailSettings.WndYPos + SettingsHelper.TailSettings.WndHeight / 2 > SystemParameters.VirtualScreenHeight)
        SettingsHelper.TailSettings.WndYPos = SystemParameters.VirtualScreenHeight - SettingsHelper.TailSettings.WndHeight;

      if(SettingsHelper.TailSettings.WndXPos + SettingsHelper.TailSettings.WndWidth / 2 > SystemParameters.VirtualScreenWidth)
        SettingsHelper.TailSettings.WndXPos = SystemParameters.VirtualScreenWidth - SettingsHelper.TailSettings.WndWidth;

      if(SettingsHelper.TailSettings.WndYPos < 0)
        SettingsHelper.TailSettings.WndYPos = 0;

      if(SettingsHelper.TailSettings.WndXPos < 0)
        SettingsHelper.TailSettings.WndXPos = 0;
    }

    private Frame CreateTabItemContent(TabItem tabItem, FileManagerData properties = null)
    {
      LOG.Trace("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

      TailLog tailWindow;

      if(properties == null)
      {
        tailWindow = new TailLog(tabControl.Items.Count, tabItem)
        {
          ActiveTab = true
        };
      }
      else
      {
        tailWindow = new TailLog(tabControl.Items.Count, tabItem, properties)
        {
          ActiveTab = true
        };
      }

      tailWindow.FileManagerDoOpenTab += FileManagerTab;
      tailWindow.ButtonSearchBox += OpenSearchBoxWindow;
      tailWindow.OnDragAndDropEvent += TailWindow_OnDragAndDropEvnt;
      tailWindow.OnIsOpenInTabControl += TailWindow_OnIsOpenInTabControl;

      Frame tabFrame = new Frame
      {
        Content = tailWindow
      };
      tabFrame.ContentRendered += OnContentRendered;
      tabItem.Content = tabFrame;

      // Update statusbar text
      StatusBarState.Content = tailWindow.GetChildState();

      if(searchBoxWindow.Visibility != Visibility.Visible)
        return (tabFrame);

      tailWindow.SearchBoxActive();
      tailWindow.WrapAround(searchBoxWindow.WrapSearch);

      return (tabFrame);
    }

    private void TailWindow_OnDragAndDropEvnt(FileManagerData fileProperties)
    {
      if(fileProperties == null)
        return;

      LOG.Trace("Drag'n'Drop get new file '{0}'", fileProperties.FileName);
      AddTabItem(fileProperties);
    }

    private void TabItemUpdateParent(TailLog page)
    {
      SetTabNotActive(page);
      page.UpdateStatusBarOnTabSelectionChange();
      page.UpdateCheckBoxOnTopOnWindowTopmost(Topmost);
    }

    private void DeleteLogFiles()
    {
      if(!Directory.Exists("logs"))
        return;

      try
      {
        var files = new DirectoryInfo("logs").GetFiles("*.log");

        foreach(var item in files.Where(f => DateTime.Now - f.LastWriteTimeUtc > TimeSpan.FromDays(LogFile.DELETE_LOG_FILES_OLDER_THAN)))
        {
          try
          {
            item.Delete();
          }
          catch
          {
            continue;
          }
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType());
      }
    }

    private void SetTabNotActive(TailLog activePage)
    {
      try
      {
        foreach(TabItem item in tabControl.Items)
        {
          if(item.Content == null || item.Content.GetType() != typeof(Frame))
            continue;

          TailLog page = GetTailLogWindow(item.Content as Frame);

          if(page == null)
            return;

          if(page.GetChildTabIndex() != activePage.GetChildTabIndex())
            page.ActiveTab = false;
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private static TailLog GetTailLogWindow(Frame tabTemplate)
    {
      if(tabTemplate == null || tabTemplate.Content == null)
        return (null);

      if(tabTemplate.Content is TailLog tabPage)
        return (tabPage);

      return (null);
    }

    private void OnExit()
    {
      SettingsHelper.TailSettings.CurrentWindowState = MainWindow.WindowState;

      if(SettingsHelper.TailSettings.CurrentWindowState == WindowState.Normal)
      {
        if(SettingsHelper.TailSettings.RestoreWindowSize)
        {
          SettingsHelper.TailSettings.WndWidth = Application.Current.MainWindow.Width;
          SettingsHelper.TailSettings.WndHeight = Application.Current.MainWindow.Height;
        }
        else
        {
          SettingsHelper.TailSettings.WndWidth = -1;
          SettingsHelper.TailSettings.WndHeight = -1;
        }

        if(SettingsHelper.TailSettings.SaveWindowPosition)
        {
          SettingsHelper.TailSettings.WndXPos = Application.Current.MainWindow.Left;
          SettingsHelper.TailSettings.WndYPos = Application.Current.MainWindow.Top;
        }
        else
        {
          SettingsHelper.TailSettings.WndXPos = -1;
          SettingsHelper.TailSettings.WndYPos = -1;
        }
      }

      SettingsHelper.SaveSettings();

      if(SettingsHelper.TailSettings.DeleteLogFiles)
        DeleteLogFiles();

      Dispose();
      currentPage = null;

      searchBoxWindow.Close();
      Application.Current.Shutdown();
    }

    private void HandleMainWindowKeys(object sender, KeyEventArgs e)
    {
      LOG.Debug("HandleMainWindowKeys Key='{0}' Keyboard='{1}' ModifierKeys='{2}'", e.Key, Keyboard.Modifiers, ModifierKeys.Control);

      // If exit while pressing ESC, than exit application
      if(SettingsHelper.TailSettings.ExitWithEscape)
      {
        if(e.Key == Key.Escape)
          OnExit();
      }

      // When pressing Control + F shows the search dialogue
      if(e.Key == Key.F && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnSearch_Click(sender, e);
      else if(e.Key == Key.F && !currentPage.TextBoxFileNameIsFocused) // When pressing F toggle filter on/off
        currentPage.FilterOnOff();

      // When pressing Control + O shows the open file dialogue
      if(e.Key == Key.O && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnOpenFile_Click(sender, e);

      // When pressing Control + Alt + M minimize main window
      if(e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) != 0 && (Keyboard.Modifiers & ModifierKeys.Alt) != 0)
        LogFile.MinimizeMainWindow();
      else if(e.Key == Key.M && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control) // When pressing Control + M shows the file manager dialogue
        currentPage.btnFileManager_Click(sender, e);

      // When pressing Control + E clear all content in Tailwindow
      if(e.Key == Key.E && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnClearTextBox_Click(sender, e);

      // When pressing Control + R start tail process
      if(e.Key == Key.R && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnStart_Click(sender, e);

      // When pressing Control + S pause tail process
      if(e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnStop_Click(sender, e);

      // When pressing Control + G show GoToLine dialogue
      if(e.Key == Key.G && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.GoToLineNumber();

      // When pressing Control + T new Tab
      if(e.Key == Key.T && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        AddTabItem();
      else if(e.Key == Key.T && !currentPage.TextBoxFileNameIsFocused) // When pressing T toggle always on top on/off
        currentPage.AlwaysOnTop();

      // When pressing Control + W close tab
      if(e.Key != Key.W || (Keyboard.Modifiers & (ModifierKeys.Control)) != ModifierKeys.Control)
        return;

      if(tabControl.SelectedItem is TabItem tab)
        RemoveTabItem(tab);
    }

    private void OpenSearchBoxWindow(object sender, EventArgs e)
    {
      if(e is FileManagerDataEventArgs)
      {
        if(e is FileManagerDataEventArgs data)
        {
          FileManagerData properties = data.GetData();
          double xPos, yPos;

          if(SettingsHelper.TailSettings.SearchWndXPos == -1.0f)
          {
            xPos = LogFile.APP_MAIN_WINDOW.Left + 50;
          }
          else
          {
            if(SettingsHelper.TailSettings.SearchWndXPos + searchBoxWindow.Width / 2 > SystemParameters.VirtualScreenWidth)
              xPos = LogFile.APP_MAIN_WINDOW.Left + 50;
            else
              xPos = SettingsHelper.TailSettings.SearchWndXPos;
          }

          if(SettingsHelper.TailSettings.SearchWndYPos == -1.0f)
          {
            yPos = LogFile.APP_MAIN_WINDOW.Top + 50;
          }
          else
          {
            if(SettingsHelper.TailSettings.SearchWndYPos + searchBoxWindow.Height / 2 > SystemParameters.VirtualScreenHeight)
              yPos = LogFile.APP_MAIN_WINDOW.Top + 50;
            else
              yPos = SettingsHelper.TailSettings.SearchWndYPos;
          }

          searchBoxWindow.Left = xPos;
          searchBoxWindow.Top = yPos;
          searchBoxWindow.SetTitle = properties.File;
        }

        searchBoxWindow.Show();
        searchBoxWindow.Owner = Window.GetWindow(this);
        currentPage.SearchBoxActive();
      }
    }

    private void HideSearchBoxEvent(object sender, EventArgs e)
    {
      foreach(TabItem item in tabControl.Items)
      {
        if(item.Content != null && item.Content.GetType() == typeof(Frame))
        {
          var page = GetTailLogWindow(item.Content as Frame);

          if(page == null)
            continue;

          page.SearchBoxInactive();
        }
      }
    }

    private void FindNextEvent(object sender, EventArgs e)
    {
      currentPage?.FindNext(e as SearchData);
    }

    private void CoundSearchEvent(object sender, EventArgs e)
    {
      if(currentPage != null)
        searchBoxWindow.SetStatusBarSearchCountText(currentPage.SearchCount());
    }

    private void FindWhatTextChangedEvent(object sender, EventArgs e)
    {
      currentPage?.FindWhatTextChanged();
    }

    private void WrapAroundEvent(object sender, EventArgs e)
    {
      if(e is WrapAroundBool wrap)
      {
        foreach(TabItem item in tabControl.Items)
        {
          if(item.Content != null && item.Content.GetType() == typeof(Frame))
          {
            var page = GetTailLogWindow(item.Content as Frame);

            if(page == null)
              continue;

            page.WrapAround(wrap != null && wrap.Wrap);
          }
        }
      }
    }

    private void BookmarkLineEvent(object sender, EventArgs e)
    {
      // TODO review me!!
      if(e is BookmarkLineBool bookmarkLine)
      {
        foreach(TabItem item in tabControl.Items)
        {
          if(item.Content != null || item.Content.GetType() == typeof(Frame))
          {
            var page = GetTailLogWindow(item.Content as Frame);

            if(page == null)
              continue;

            page.BookmarkLine(bookmarkLine != null || bookmarkLine.BookmarkLine);
          }
        }
      }
    }

    private void TailWindow_OnIsOpenInTabControl(TabItem tabItem)
    {
      tabControl.SelectedItem = tabItem;
    }

    private void SelectTabItem(string tabHeader)
    {
      TailForWinTabItem selectedTab = null;

      foreach(TailForWinTabItem item in tabControl.Items)
      {
        if(item.Header.ToString() == tabHeader)
        {
          selectedTab = item;
          break;
        }
      }

      if(selectedTab != null)
        tabControl.SelectedItem = selectedTab;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      handled = false;

      switch(msg)
      {
      case NativeMethods.WM_ENTERSIZEMOVE:

        hasFocus = true;
        break;

      case NativeMethods.WM_EXITSIZEMOVE:

        hasFocus = false;
        DragWindowManager.Instance.DragEnd(this);
        break;

      case NativeMethods.WM_MOVE:

        if(hasFocus)
          DragWindowManager.Instance.DragMove(this);
        break;

      case NativeMethods.WM_GETMINMAXINFO:

        WmGetMinMaxInfo(hwnd, lParam);
        handled = true;
        break;

      case NativeMethods.WM_WINDOWPOSCHANGING:

        WINDOWPOS pos = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

        if((pos.flags & (int) (SWP.NOMOVE)) != 0)
          return (IntPtr.Zero);

        Window wnd = (Window) HwndSource.FromHwnd(hwnd).RootVisual;

        if(wnd == null)
          return (IntPtr.Zero);

        bool changedPos = false;

        if(pos.cx < MinWidth)
        {
          pos.cx = (int) MinWidth;
          changedPos = true;
        }

        if(pos.cy < MinHeight)
        {
          pos.cy = (int) MinHeight;
          changedPos = true;
        }

        if(!changedPos)
          return (IntPtr.Zero);

        Marshal.StructureToPtr(pos, lParam, true);
        handled = true;
        break;

      }
      return (IntPtr.Zero);
    }

    /// <summary>
    /// This is required, when the window has own WPF style, it's maximized, that the window hides taskbar
    /// The reason is, the window style <c>None</c>
    /// </summary>
    /// <param name="hwnd">Handle of window</param>
    /// <param name="lParam">Low parameter</param>
    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
      MINMAXINFO mmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

      // Adjust the maximized size and position to fit the work area of the correct monitor
      int MONITOR_DEFAULTTONEAREST = 0x00000002;
      IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

      if(monitor != IntPtr.Zero)
      {
        MonitorInfo monitorInfo = new MonitorInfo();

        NativeMethods.GetMonitorInfo(monitor, monitorInfo);

        Data.Native.Rect rcWorkArea = monitorInfo.rcWork;
        Data.Native.Rect rcMonitorArea = monitorInfo.rcMonitor;
        mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
        mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
        mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
        mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
      }

      Marshal.StructureToPtr(mmi, lParam, true);
    }

    #endregion
  }
}
