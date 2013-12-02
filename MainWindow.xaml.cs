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
  public partial class MainWindow: Window, IDisposable
  {
    private WinTrayIcon trayIcon;
    private List<TabItem> tailTabItems;
    private TabItem tabAdd;
    private int tabCount;
    private TailLog currentPage;
    private SearchDialog searchBoxWindow;


    public void Dispose ()
    {
      if (trayIcon != null)
      {
        trayIcon.Dispose ( );
        trayIcon = null;
      }

      foreach (TabItem item in tailTabItems)
      {
        if (item.Content != null && item.Content.GetType ( ) == typeof (Frame))
        {
          TailLog page = GetTailLogWindow (item.Content as Frame);

          if (page != null)
            page.StopThread ( );
        }
      }
    }

    public MainWindow ()
    {
      InitializeComponent ( );

      // TrayIcon stuff
      Stream stream = Application.GetResourceStream (new Uri ("pack://application:,,,/TailForWin;component/Res/Main.ico")).Stream;
      Icon icon = new Icon (stream);
      trayIcon = new WinTrayIcon (icon, string.Format ("{0} {1}", LogFile.APPLICATION_CAPTION, Application.Current.FindResource ("TrayIconReady")));

      trayIcon.NotifyIcon.DoubleClick += DoubleClickNotifyIcon;      

      SetWindowSettings ( );

      tailTabItems = new List<TabItem> ( );
      tabCount = 0;

      tabAdd = new TabItem ( ) { Header = "+", Name = "AddChildTab" };
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
    public ComboBox StatusBarEncodeCB
    {
      get
      {
        return (cbStsEncoding);
      }
    }

    #endregion

    #region ClickEvents

    private void DoubleClickNotifyIcon (object sender, EventArgs e)
    {
      LogFile.BringMainWindoToFront ( );
    }

    private void tabControlTail_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      TabItem tab = tabControlTail.SelectedItem as TabItem;

      if (tab == null)
        return;

      if (tab.Equals (tabAdd))
      {
        TabItem newTab = AddTailTab ( );
        tabControlTail.SelectedItem = newTab;
      }
      else
      {
        TailLog page = GetTailLogWindow (tab.Content as Frame);

        if (page != null)
        {
          stsBarState.Content = page.GetChildState ( );
          page.ActiveTab = true;

          currentPage = page;
          TabItemUpdateParent (page);
        }
      }
    }

    private void btnDelete_Click (object sender, RoutedEventArgs e)
    {
      string tabName = (sender as Button).CommandParameter.ToString ( );
      TabItem tab = tabControlTail.Items.Cast<TabItem> ( ).Where (i => i.Name.Equals (tabName)).SingleOrDefault ( );

      if (tab != null)
      {
        if (tailTabItems.Count < 3)
          MessageBox.Show (Application.Current.FindResource ("LastTab") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
        else if (MessageBox.Show (string.Format ("{0} '{1}'?", Application.Current.FindResource ("QRemoveTab"), tab.Header), LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          TabItem selectedTab = tabControlTail.SelectedItem as TabItem;

          tabControlTail.DataContext = null;

          tailTabItems.Remove (tab);

          tabControlTail.DataContext = tailTabItems;

          TailLog page = GetTailLogWindow (tab.Content as Frame);

          if (page != null)
            page.StopThread ( );

          // select previously selected tab. if that is removed then select first tab
          if (selectedTab == null || selectedTab.Equals (tab))
            selectedTab = tailTabItems[0];

          tabControlTail.SelectedItem = selectedTab;
        }
      }
    }

    private void cbStsEncoding_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      if (currentPage != null)
        currentPage.UpdateFileEncoding ((Encoding) cbStsEncoding.SelectedItem);
    }

    #endregion

    #region Events

    private void tabControlTail_Drop (object sender, DragEventArgs e)
    {
      currentPage.DropHelper (sender, e);
    }

    private void tabControlTail_DragEnter (object sender, DragEventArgs e)
    {
      currentPage.DragEnterHelper (sender, e);
    }

    private void Window_Closing (object sender, CancelEventArgs e)
    {
      OnExit ( );
    }

    private void OnContentRendered (object sender, EventArgs e)
    {
      Frame frame = sender as Frame;

      if (frame.Content != null)
      {
        TailLog page = frame.Content as TailLog;

        if (page != null)
        {
          currentPage = page;
          TabItemUpdateParent (page);
        }
      }
    }

    private void HandleMainWindowKeys (object sender, KeyEventArgs e)
    {
      // If exit while pressing ESC, than exit application
      if (SettingsHelper.TailSettings.ExitWithEscape == true)
      {
        if (e.Key == Key.Escape)
          OnExit ( );
      }

      // When pressing Control + F shows the search dialog
      if (e.Key == Key.F && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnSearch_Click (sender, e);

      // When pressing Control + O shows the open file dialog
      if (e.Key == Key.O && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnOpenFile_Click (sender, e);

      // When pressing Control + M shows the file manager dialog
      if (e.Key == Key.M && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnFileManager_Click (sender, e);

      // When pressing Control + E clear all content in Tailwindow
      if (e.Key == Key.E && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnClearTextBox_Click (sender, e);

      // When pressing Control + T start tail process
      if (e.Key == Key.T && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnStart_Click (sender, e);

      // When pressing Control + S pause tail process
      if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
        currentPage.btnStop_Click (sender, e);
    }

    private void OpenSearchBoxWindow (object sender, EventArgs e)
    {
      if (e.GetType ( ) == typeof (FileManagerDataEventArgs))
      {
        FileManagerDataEventArgs data = e as FileManagerDataEventArgs;
        FileManagerData properties = data.GetData ( );
        double xPos, yPos;

        if (SettingsHelper.TailSettings.SearchWndXPos == -1)
          xPos = LogFile.APP_MAIN_WINDOW.Left + 50;
        else
          xPos = SettingsHelper.TailSettings.SearchWndXPos;

        if (SettingsHelper.TailSettings.SearchWndYPos == -1)
          yPos = LogFile.APP_MAIN_WINDOW.Top + 50;
        else
          yPos = SettingsHelper.TailSettings.SearchWndYPos;

        searchBoxWindow.Left = xPos;
        searchBoxWindow.Top = yPos;
        searchBoxWindow.SetTitle = properties.File;
        searchBoxWindow.Show ( );
        currentPage.SearchBoxActive ( );
      }
    }

    private void HideSearchBoxEvent (object sender, EventArgs e)
    {
      currentPage.SearchBoxInactive ( );
    }

    private void FindNextEvent (object sender, EventArgs e)
    {
      currentPage.FindNext (e as SearchData);
    }

    private void CoundSearchEvent (object sender, EventArgs e)
    {
      searchBoxWindow.SetStatusBarSearchCountText (currentPage.SearchCount ( ));
    }

    private void FindWhatTextChangedEvent (object sender, EventArgs e)
    {
      currentPage.FindWhatTextChanged ( );
    }

    #endregion

    #region Helperfunctions
   
    public void FileManagerTab (object sender, EventArgs e)
    {
      if (e.GetType ( ) == typeof (FileManagerDataEventArgs))
      {
        FileManagerDataEventArgs properties = e as FileManagerDataEventArgs;
        FileManagerData item = properties.GetData ( );
        TabItem newTab = AddTailTab (item);
        tabControlTail.SelectedItem = newTab;
      }
    }

    private void SetWindowSettings ()
    {
      SettingsHelper.ReadSettings ( );
      Title = LogFile.APPLICATION_CAPTION;
      LogFile.InitObservableCollectionsRRTPFE ( );

      cbStsEncoding.DataContext = LogFile.FileEncoding;
      cbStsEncoding.DisplayMemberPath = "HeaderName";

      Topmost = SettingsHelper.TailSettings.AlwaysOnTop;

      PreviewKeyDown += HandleMainWindowKeys;

      if (SettingsHelper.TailSettings.RestoreWindowSize == true)
      {
        if (SettingsHelper.TailSettings.WndWidth != -1)
          Application.Current.MainWindow.Width = SettingsHelper.TailSettings.WndWidth;
        if (SettingsHelper.TailSettings.WndHeight != -1)
          Application.Current.MainWindow.Height = SettingsHelper.TailSettings.WndHeight;
      }

      if (SettingsHelper.TailSettings.SaveWindowPosition == true)
      {
        if (SettingsHelper.TailSettings.WndYPos != -1)
          Application.Current.MainWindow.Top = SettingsHelper.TailSettings.WndYPos;
        if (SettingsHelper.TailSettings.WndXPos != -1)
          Application.Current.MainWindow.Left = SettingsHelper.TailSettings.WndXPos;
      }

      searchBoxWindow = new SearchDialog ( );

      // EventHandlers for searchBoxWindow
      searchBoxWindow.FindNextEvent += FindNextEvent;
      searchBoxWindow.CountSearchEvent += CoundSearchEvent;
      searchBoxWindow.HideSearchBox += HideSearchBoxEvent;
      searchBoxWindow.FindTextChanged += FindWhatTextChangedEvent;

      SoundPlay.InitSoundPlay (SettingsHelper.TailSettings.AlertSettings.SoundFileNameFullPath);
    }

    private void TabItemUpdateParent (TailLog page)
    {
      SetTabNotActive (page);
      page.UpdateStatusBarOnTabSelectionChange ( );
      page.UpdateCheckBoxOnTopOnWindowTopmost (Topmost);
    }

    private TailLog GetTailLogWindow (Frame tabTemplate)
    {
      TailLog tabPage = tabTemplate.Content as TailLog;

      if (tabPage != null)
        return (tabPage);
      else
        return (null);
    }

    private void OnExit ()
    {
      if (SettingsHelper.TailSettings.RestoreWindowSize == true)
      {
        SettingsHelper.TailSettings.WndWidth = Application.Current.MainWindow.Width;
        SettingsHelper.TailSettings.WndHeight = Application.Current.MainWindow.Height;
      }
      else
      {
        SettingsHelper.TailSettings.WndWidth = -1;
        SettingsHelper.TailSettings.WndHeight = -1;
      }

      if (SettingsHelper.TailSettings.SaveWindowPosition == true)
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

      Application.Current.Shutdown ( );
    }

    private TabItem AddTailTab (FileManagerData properties = null)
    {
      if (tailTabItems.Count <= LogFile.MAX_TAB_CHILDS)
      {
        tabControlTail.DataContext = null;
        int count = tailTabItems.Count;

        TabItem tabItem = new TabItem ( ) 
        { 
          Header = LogFile.TABBAR_CHILD_EMPTY_STRING, 
          Name = string.Format ("TabIndex_{0}", tabCount), 
          HeaderTemplate = tabControlTail.FindResource ("TabHeader") as DataTemplate 
        };

        TailLog tailWindow;

        if (properties == null)
          tailWindow = new TailLog (tabCount, tabItem) { ActiveTab = true };
        else
          tailWindow = new TailLog (tabCount, tabItem, properties) { ActiveTab = true };

        tailWindow.FileManagerDoOpenTab += FileManagerTab;
        tailWindow.ButtonSearchBox += OpenSearchBoxWindow;

        Frame tabFrame = new Frame ( ) { Content = tailWindow };
        tabFrame.ContentRendered += OnContentRendered;
        tabItem.Content = tabFrame;

        // Update statusbar text
        stsBarState.Content = tailWindow.GetChildState ( );

        tailTabItems.Insert (count - 1, tabItem);
        tabControlTail.DataContext = tailTabItems;
        tabCount++;
        
        return (tabItem);
      } 
      else
      {
        MessageBox.Show (Application.Current.FindResource ("HCloseTab") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);

        return (tailTabItems[tailTabItems.Count - 2]);
      }
    }

    private void SetTabNotActive (TailLog activePage)
    {
      foreach (TabItem item in tailTabItems)
      {
        if (item.Content != null && item.Content.GetType ( ) == typeof (Frame))
        {
          TailLog page = GetTailLogWindow (item.Content as Frame);

          if (page.GetChildTabIndex ( ) != activePage.GetChildTabIndex ( ))
            page.ActiveTab = false;
        }
      }
    }

    #endregion
  }
}
