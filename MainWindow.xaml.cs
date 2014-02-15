using System.Windows;
using System.Windows.Controls;
using TailForWin.Template;
using TailForWin.Data;
using TailForWin.Utils;
using TailForWin.Controller;
using System.IO;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Input;
using System.Text;


namespace TailForWin
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : IDisposable
  {
    private WinTrayIcon trayIcon;
    private readonly List<TabItem> tailTabItems;
    private readonly TabItem tabAdd;
    private int tabCount;
    private TailLog currentPage;
    private SearchDialog searchBoxWindow;
    private bool ctrlTabKey;


    public void Dispose ()
    {
      if (trayIcon != null)
      {
        trayIcon.Dispose ( );
        trayIcon = null;
      }

      foreach (TailLog page in tailTabItems.Where (item => item.Content != null && item.Content.GetType ( ) == typeof (Frame)).Select (item => GetTailLogWindow (item.Content as Frame)).Where (page => page != null))
        page.StopThread ( );

      if (currentPage == null)
        return;

      currentPage.Dispose ( );
      currentPage = null;
    }

    public MainWindow ()
    {
      InitializeComponent ( );

      // TrayIcon stuff
      var streamResourceInfo = Application.GetResourceStream (new Uri ("pack://application:,,,/TailForWin;component/Res/Main.ico"));
     
      if (streamResourceInfo != null)
      {
        Stream stream = streamResourceInfo.Stream;
        Icon icon = new Icon (stream);
        trayIcon = new WinTrayIcon (icon, string.Format ("{0} {1}", LogFile.APPLICATION_CAPTION, Application.Current.FindResource ("TrayIconReady")));
      }

      trayIcon.NotifyIcon.DoubleClick += DoubleClickNotifyIcon;

      tabControlTail.PreviewKeyDown += tabControlTail_PreviewKeyDown;
      tabControlTail.PreviewKeyUp += tabControlTail_PreviewKeyUp;

      SetWindowSettings ( );

      tailTabItems = new List<TabItem> ( );
      tabCount = 0;

      tabAdd = new TabItem
      {
        Header = "+",
        Name = "AddChildTab",
        Style = (Style) FindResource ("TabItemStopStyle")
      };

      tailTabItems.Add (tabAdd);

      AddTailTab ( );
      tabControlTail.SelectedIndex = 0;

      // Important for command line parameter!
      if (LogFile.APP_MAIN_WINDOW == null)
        LogFile.APP_MAIN_WINDOW = (Application.Current.MainWindow as MainWindow);
    }

    #region Properties

    /// <summary>
    /// Set topmost
    /// </summary>
    public bool MainWindowTopmost
    {
      get
      {
        return (Topmost);
      }
      set
      {
        Topmost = value;
      }
    }

    /// <summary>
    /// Set statusbar state item
    /// </summary>
    public StatusBarItem StatusBarState
    {
      get
      {
        return (stsBarState);
      }
    }

    /// <summary>
    /// Set statusbar encoding item
    /// </summary>
    public StatusBarItem StatusBarEncoding
    {
      get
      {
        return (stsEncoding);
      }
    }

    /// <summary>
    /// Set statusbar lines read
    /// </summary>
    public StatusBarItem StatusBarLinesRead
    {
      get
      {
        return (stsLinesRead);
      }
    }

    /// <summary>
    /// Set statubar encode combobox (cbStsEncoding)
    /// </summary>
    public ComboBox StatusBarEncodeCb
    {
      get
      {
        return (cbStsEncoding);
      }
    }

    #endregion

    #region ClickEvents

    private static void DoubleClickNotifyIcon (object sender, EventArgs e)
    {
      LogFile.BringMainWindowToFront ( );
    }

    private void tabControlTail_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!(e.Source is TabControl)) 
        return;

      TabItem tab = tabControlTail.SelectedItem as TabItem;

      if (tab == null)
        return;

      if (tab.Equals (tabAdd) && !ctrlTabKey)
      {
        TabItem newTab = AddTailTab ( );
        tabControlTail.SelectedItem = newTab;
      }
      else
      {
        if (tab.Equals (tabAdd) && ctrlTabKey)
        {
          tab = tailTabItems[0];
          tabControlTail.SelectedItem = tab;
        }

        TailLog page = GetTailLogWindow (tab.Content as Frame);

        if (page == null)
          return;

        if (currentPage != null && (page.GetChildTabIndex ( ) == currentPage.GetChildTabIndex ( )))
          return;

        stsBarState.Content = page.GetChildState ( );
        page.ActiveTab = true;

        if (searchBoxWindow.Visibility == Visibility.Visible)
        {
          page.SearchBoxActive ( );
          page.WrapAround (searchBoxWindow.WrapSearch);
          FindWhatTextChangedEvent (this, EventArgs.Empty);
          searchBoxWindow.SetTitle = page.FileManagerProperties.File;
        }

        currentPage = page;
        TabItemUpdateParent (page);
      }
    }

    private void btnDelete_Click (object sender, RoutedEventArgs e)
    {
      var button = sender as Button;

      if (button != null)
        RemoveTab (button.CommandParameter.ToString ( ));
    }

    private void cbStsEncoding_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (currentPage != null)
        currentPage.UpdateFileEncoding ((Encoding) cbStsEncoding.SelectedItem);
    }

    #endregion

    #region Events

    private void tabControlTail_PreviewKeyDown (object sender, KeyEventArgs e)
    {
      if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.Tab)
        ctrlTabKey = true;
      if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && (Keyboard.Modifiers & ModifierKeys.Shift) != 0 && e.Key == Key.Tab)
        e.Handled = true;
    }

    private void tabControlTail_PreviewKeyUp (object sender, KeyEventArgs e)
    {
      ctrlTabKey = false;
    }

    private void tabControlTail_Drop (object sender, DragEventArgs e)
    {
      if (currentPage != null)
        currentPage.DropHelper (sender, e);
    }

    private void tabControlTail_DragEnter (object sender, DragEventArgs e)
    {
      if (currentPage != null)
        currentPage.DragEnterHelper (sender, e);
    }

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      if (SettingsHelper.TailSettings.AutoUpdate)
      {
        AutoUpdate.Init ( );
      }
    }

    private void Window_Closing (object sender, CancelEventArgs e)
    {
      OnExit ( );
    }

    private void OnContentRendered (object sender, EventArgs e)
    {
      Frame frame = sender as Frame;

      if (frame == null || frame.Content == null) 
        return;

      TailLog page = frame.Content as TailLog;

      if (page == null) 
        return;

      currentPage = page;
      TabItemUpdateParent (page);
    }

    private void HandleMainWindowKeys (object sender, KeyEventArgs e)
    {
      // If exit while pressing ESC, than exit application
      if (SettingsHelper.TailSettings.ExitWithEscape)
      {
        if (e.Key == Key.Escape)
          OnExit ( );
      }

      // When pressing Control + F shows the search dialogue
      if (e.Key == Key.F && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnSearch_Click (sender, e);
      else if (e.Key == Key.F) // When pressing F toggle filter on/off
        currentPage.FilterOnOff ( );

      // When pressing Control + O shows the open file dialogue
      if (e.Key == Key.O && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnOpenFile_Click (sender, e);

      // When pressing Control + Alt + M minimize main window
      if (e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) != 0 && (Keyboard.Modifiers & ModifierKeys.Alt) != 0)
        LogFile.MinimizeMainWindow ( );
      else if (e.Key == Key.M && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control) // When pressing Control + M shows the file manager dialogue
        currentPage.btnFileManager_Click (sender, e);

      // When pressing Control + E clear all content in Tailwindow
      if (e.Key == Key.E && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnClearTextBox_Click (sender, e);

      // When pressing Control + R start tail process
      if (e.Key == Key.R && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnStart_Click (sender, e);

      // When pressing Control + S pause tail process
      if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnStop_Click (sender, e);

      // When pressing Control + G show GoToLine dialogue
      if (e.Key == Key.G && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.GoToLineNumber ( );

      // When pressing Control + T new Tab
      if (e.Key == Key.T && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
      {
        TabItem newTab = AddTailTab ( );
        tabControlTail.SelectedItem = newTab;
      }
      else if (e.Key == Key.T) // When pressing T toggle always on top on/off
        currentPage.AlwaysOnTop ( );

      // When pressing Control + W close tab
      if (e.Key != Key.W || (Keyboard.Modifiers & (ModifierKeys.Control)) != ModifierKeys.Control) 
        return;

      TabItem tab = tabControlTail.SelectedItem as TabItem;
        
      if (tab != null) 
        RemoveTab (tab.Name);
    }

    private void OpenSearchBoxWindow (object sender, EventArgs e)
    {
      if (e.GetType() != typeof (FileManagerDataEventArgs)) 
        return;

      FileManagerDataEventArgs data = e as FileManagerDataEventArgs;

      if (data != null)
      {
        FileManagerData properties = data.GetData ( );
        double xPos, yPos;

        if (SettingsHelper.TailSettings.SearchWndXPos == -1.0f)
          xPos = LogFile.APP_MAIN_WINDOW.Left + 50;
        else
          xPos = SettingsHelper.TailSettings.SearchWndXPos;

        if (SettingsHelper.TailSettings.SearchWndYPos == -1.0f)
          yPos = LogFile.APP_MAIN_WINDOW.Top + 50;
        else
          yPos = SettingsHelper.TailSettings.SearchWndYPos;

        searchBoxWindow.Left = xPos;
        searchBoxWindow.Top = yPos;
        searchBoxWindow.SetTitle = properties.File;
      }
      searchBoxWindow.Show ( );

      foreach (TailLog page in (from item in tailTabItems
                                where item.Content != null && item.Content.GetType ( ) == typeof (Frame)
                                select GetTailLogWindow (item.Content as Frame)).Where (page => page != null))
        page.SearchBoxActive ( );
    }

    private void HideSearchBoxEvent (object sender, EventArgs e)
    {
      if (currentPage != null)
        currentPage.SearchBoxInactive ( );
    }

    private void FindNextEvent (object sender, EventArgs e)
    {
      if (currentPage != null)
        currentPage.FindNext (e as SearchData);
    }

    private void CoundSearchEvent (object sender, EventArgs e)
    {
      if (currentPage != null)
        searchBoxWindow.SetStatusBarSearchCountText (currentPage.SearchCount ( ));
    }

    private void FindWhatTextChangedEvent (object sender, EventArgs e)
    {
      if (currentPage != null)
        currentPage.FindWhatTextChanged ( );
    }

    private void WrapAroundEvent (object sender, EventArgs e)
    {
      WrapAroundBool wrap = e as WrapAroundBool;

      foreach (TailLog page in from item in tailTabItems
                               where item.Content != null && item.Content.GetType ( ) == typeof (Frame)
                               select GetTailLogWindow (item.Content as Frame))
        page.WrapAround (wrap != null && wrap.Wrap);
    }

    private void BookmarkLineEvent (object sender, EventArgs e)
    {
      BookmarkLineBool bookmarkLine = e as BookmarkLineBool;

      foreach (TailLog page in from item in tailTabItems
                               where item.Content != null && item.Content.GetType ( ) == typeof (Frame)
                               select GetTailLogWindow (item.Content as Frame))
        page.BookmarkLine (bookmarkLine != null && bookmarkLine.BookmarkLine);
    }

    #endregion

    #region Helperfunctions

    public void FileManagerTab (object sender, EventArgs e)
    {
      if (e.GetType() != typeof (FileManagerDataEventArgs)) 
        return;

      FileManagerDataEventArgs properties = e as FileManagerDataEventArgs;

      if (properties != null)
      {
        FileManagerData item = properties.GetData ( );
        TabItem newTab = AddTailTab (item);
        tabControlTail.SelectedItem = newTab;
      }
    }

    private void SetWindowSettings ()
    {
      SettingsHelper.ReadSettings ( );
      Title = LogFile.APPLICATION_CAPTION;
      LogFile.InitObservableCollectionsRrtpfe ( );

      cbStsEncoding.DataContext = LogFile.FileEncoding;
      cbStsEncoding.DisplayMemberPath = "HeaderName";

      Topmost = SettingsHelper.TailSettings.AlwaysOnTop;

      PreviewKeyDown += HandleMainWindowKeys;

      if (SettingsHelper.TailSettings.RestoreWindowSize)
      {
        if (SettingsHelper.TailSettings.WndWidth != -1.0f)
          Application.Current.MainWindow.Width = SettingsHelper.TailSettings.WndWidth;
        if (SettingsHelper.TailSettings.WndHeight != -1.0f)
          Application.Current.MainWindow.Height = SettingsHelper.TailSettings.WndHeight;
      }

      if (SettingsHelper.TailSettings.SaveWindowPosition)
      {
        if (SettingsHelper.TailSettings.WndYPos != -1.0f)
          Application.Current.MainWindow.Top = SettingsHelper.TailSettings.WndYPos;
        if (SettingsHelper.TailSettings.WndXPos != -1.0f)
          Application.Current.MainWindow.Left = SettingsHelper.TailSettings.WndXPos;
      }

      searchBoxWindow = new SearchDialog ( );

      // EventHandlers for searchBoxWindow
      searchBoxWindow.FindNextEvent += FindNextEvent;
      searchBoxWindow.CountSearchEvent += CoundSearchEvent;
      searchBoxWindow.HideSearchBox += HideSearchBoxEvent;
      searchBoxWindow.FindTextChanged += FindWhatTextChangedEvent;
      searchBoxWindow.WrapAround += WrapAroundEvent;
      searchBoxWindow.BookmarkLine += BookmarkLineEvent;
    }

    private void TabItemUpdateParent (TailLog page)
    {
      SetTabNotActive (page);
      page.UpdateStatusBarOnTabSelectionChange ( );
      page.UpdateCheckBoxOnTopOnWindowTopmost (Topmost);
    }

    private static TailLog GetTailLogWindow (Frame tabTemplate)
    {
      TailLog tabPage = tabTemplate.Content as TailLog;

      return (tabPage != null ? (tabPage) : (null));
    }

    private void OnExit ()
    {
      if (SettingsHelper.TailSettings.RestoreWindowSize)
      {
        SettingsHelper.TailSettings.WndWidth = Application.Current.MainWindow.Width;
        SettingsHelper.TailSettings.WndHeight = Application.Current.MainWindow.Height;
      }
      else
      {
        SettingsHelper.TailSettings.WndWidth = -1;
        SettingsHelper.TailSettings.WndHeight = -1;
      }

      if (SettingsHelper.TailSettings.SaveWindowPosition)
      {
        SettingsHelper.TailSettings.WndXPos = Application.Current.MainWindow.Left;
        SettingsHelper.TailSettings.WndYPos = Application.Current.MainWindow.Top;
      }
      else
      {
        SettingsHelper.TailSettings.WndXPos = -1;
        SettingsHelper.TailSettings.WndYPos = -1;
      }

      SettingsHelper.SaveSettings ( );
      Dispose ( );
      currentPage = null;

      searchBoxWindow.Close ( );

      ErrorLog.StopLog ( );
      Application.Current.Shutdown ( );
    }

    private TabItem AddTailTab (FileManagerData properties = null)
    {
      if (tailTabItems.Count <= LogFile.MAX_TAB_CHILDS)
      {
        tabControlTail.DataContext = null;
        int count = tailTabItems.Count;

        TabItem tabItem = new TabItem
        {
          Header = LogFile.TABBAR_CHILD_EMPTY_STRING,
          Name = string.Format ("TabIndex_{0}", tabCount),
          HeaderTemplate = tabControlTail.FindResource ("TabHeader") as DataTemplate,
          Style = (Style) FindResource ("TabItemStopStyle")
        };

        TailLog tailWindow;

        if (properties == null)
          tailWindow = new TailLog (tabCount, tabItem)
          {
            ActiveTab = true
          };
        else
          tailWindow = new TailLog (tabCount, tabItem, properties)
          {
            ActiveTab = true
          };

        tailWindow.FileManagerDoOpenTab += FileManagerTab;
        tailWindow.ButtonSearchBox += OpenSearchBoxWindow;

        Frame tabFrame = new Frame
        {
          Content = tailWindow
        };
        tabFrame.ContentRendered += OnContentRendered;
        tabItem.Content = tabFrame;

        // Update statusbar text
        stsBarState.Content = tailWindow.GetChildState ( );

        tailTabItems.Insert (count - 1, tabItem);
        tabControlTail.DataContext = tailTabItems;
        tabCount++;

        if (searchBoxWindow.Visibility != Visibility.Visible) 
          return (tabItem);

        tailWindow.SearchBoxActive ( );
        tailWindow.WrapAround (searchBoxWindow.WrapSearch);

        return (tabItem);
      }

      MessageBox.Show (Application.Current.FindResource ("HCloseTab") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);

      return (tailTabItems[tailTabItems.Count - 2]);
    }

    private void SetTabNotActive (TailLog activePage)
    {
      foreach (
        TailLog page in
          tailTabItems.Where (item => item.Content != null && item.Content.GetType ( ) == typeof (Frame))
            .Select (item => GetTailLogWindow (item.Content as Frame))
            .Where (page => page.GetChildTabIndex ( ) != activePage.GetChildTabIndex ( )))
        page.ActiveTab = false;
    }

    private void RemoveTab (string tabName)
    {
      TabItem tab = tabControlTail.Items.Cast<TabItem> ( ).SingleOrDefault (i => i.Name.Equals (tabName));

      if (tab == null) 
        return;
      if (tailTabItems.Count < 3)
        MessageBox.Show (Application.Current.FindResource ("LastTab") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
      else if (MessageBox.Show (string.Format ("{0} '{1}'?", Application.Current.FindResource ("QRemoveTab"), tab.Header), LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        TabItem selectedTab = tabControlTail.SelectedItem as TabItem;

        tabControlTail.DataContext = null;

        tailTabItems.Remove (tab);

        tabControlTail.DataContext = tailTabItems;

        TailLog page = GetTailLogWindow (tab.Content as Frame);
        FileManagerHelper item = LogFile.FmHelper.SingleOrDefault (x => x.ID == page.FileManagerProperties.ID);

        if (item != null)
          LogFile.FmHelper.Remove (item);

        if (page != null)
          page.StopThread ( );

        // select previously selected tab. if that is removed then select first tab
        if (selectedTab == null || selectedTab.Equals (tab))
          selectedTab = tailTabItems[0];

        tabControlTail.SelectedItem = selectedTab;
      }
    }

    #endregion
  }
}
