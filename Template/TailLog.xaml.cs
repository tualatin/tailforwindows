using System.Windows;
using System.Windows.Controls;
using TailForWin.Data;
using System;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using TailForWin.Utils;
using TailForWin.Controller;
using System.Windows.Media;
using System.IO;
using System.Text;
using System.Windows.Threading;
using TailForWin.Template.TextEditor.Converter;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for TailLog.xaml
  /// </summary>
  public partial class TailLog: Page, IDisposable
  {
    private TailLogData tabProperties;
    private BackgroundWorker tailWorker;
    private int childTabIndex;
    private TabItem childTabItem;
    private string childTabState;
    private string oldFileName = string.Empty;
    private FileReader myReader;
    private bool stopThread;
    private bool isInit = false;

    /// <summary>
    /// FileManager open new tab event handler
    /// </summary>
    public event EventHandler FileManagerDoOpenTab;

    /// <summary>
    /// Open the search box window event
    /// </summary>
    public event EventHandler ButtonSearchBox;


    public void Dispose ()
    {
      if (tailWorker != null)
      {
        tailWorker.Dispose ( );
        tailWorker = null;
      }

      myReader.Dispose ( );
      tabProperties.Dispose ( );
    }

    /// <summary>
    /// Tab with properties from FileManager
    /// </summary>
    /// <param name="childTabIndex">Reference index</param>
    /// <param name="tabItem">Reference tab control parent</param>
    /// <param name="fileManagerProperties">Settings from FileManager</param>
    public TailLog (int childTabIndex, TabItem tabItem, FileManagerData fileManagerProperties)
    {
      InitializeComponent ( );

      tabProperties = new TailLogData ( )
      {
        Wrap = fileManagerProperties.Wrap, 
        KillSpace = fileManagerProperties.KillSpace,
        FileName = fileManagerProperties.FileName,
        FontType = fileManagerProperties.FontType,
        RefreshRate = fileManagerProperties.RefreshRate,
        ThreadPriority = fileManagerProperties.ThreadPriority,
        ListOfFilter = fileManagerProperties.ListOfFilter,
        Timestamp = fileManagerProperties.Timestamp,
        FileEncoding = fileManagerProperties.FileEncoding
      };

      InitTailLog (childTabIndex, tabItem);

      textBoxFileName.Text = tabProperties.FileName;
    }

    /// <summary>
    /// New tab with default settings
    /// </summary>
    /// <param name="childTabIndex">Reference index</param>
    /// <param name="tabItem">Reference  tab control parent</param>
    public TailLog (int childTabIndex, TabItem tabItem)
    {
      InitializeComponent ( );

      System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;

      if (textBlockTailLog.TextEditorFontStyle == FontStyles.Italic)
        fs = System.Drawing.FontStyle.Italic;
      if (textBlockTailLog.TextEditorFontWeight == FontWeights.Bold)
        fs |= System.Drawing.FontStyle.Bold;

      System.Drawing.Font textBox = new System.Drawing.Font (textBlockTailLog.FontFamily.Source, (float) textBlockTailLog.FontSize, fs);

      tabProperties = new TailLogData ( )
      {
        Wrap = false,
        KillSpace = false,
        FontType = textBox,
        RefreshRate = SettingsHelper.TailSettings.DefaultRefreshRate,
        ThreadPriority = SettingsHelper.TailSettings.DefaultThreadPriority,
        ListOfFilter = new System.Collections.Generic.List<Filter> ( ),
        FileEncoding = null
      };

      InitTailLog (childTabIndex, tabItem);
    }

    /// <summary>
    /// Is tab active or not
    /// </summary>
    public bool ActiveTab
    {
      get;
      set;
    }

    /// <summary>
    /// Update checkBoxOnTop.IsChecked
    /// </summary>
    /// <param name="isChecked">Topmost value from parent window</param>
    public void UpdateCheckBoxOnTopOnWindowTopmost (bool isChecked)
    {
      checkBoxOnTop.IsChecked = isChecked;
    }

    /// <summary>
    /// Find search text
    /// </summary>
    /// <param name="sd">Search data structure</param>
    public void FindNext (SearchData sd)
    {
      textBlockTailLog.SearchCount = 0;
      textBlockTailLog.FindListViewItem (sd.WordToFind);
    }

    /// <summary>
    /// Get search count item
    /// </summary>
    public int SearchCount ( )
    {
      return (textBlockTailLog.GetSearchCount ( ));
    }

    #region ClickEvent
    
    private void checkBoxOnTop_Click (object sender, RoutedEventArgs e)
    {
      if (checkBoxOnTop.IsChecked == true)
        LogFile.APP_MAIN_WINDOW.MainWindowTopmost = true;
      else
        LogFile.APP_MAIN_WINDOW.MainWindowTopmost = false;
    }

    public void btnSearch_Click (object sender, RoutedEventArgs e)
    {
      if (tabProperties.File != null)
      {
        if (textBlockTailLog.LineCount != 0)
        {
          if (ButtonSearchBox != null)
          {
            FileManagerData data = new FileManagerData ( )
            {
              File = tabProperties.File,
              FileName = tabProperties.FileName
            };
            FileManagerDataEventArgs eventData = new FileManagerDataEventArgs (data);
            ButtonSearchBox (this, eventData);
            eventData.Dispose ( );
          }
        }
      }
    }

    private void btnStart_Click (object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty (tabProperties.FileName))
      {
        textBlockTailLog.AppendText (string.Format ("{0} - {1}", DateTime.Now, Application.Current.FindResource ("NoFilenameEntered")));
        textBlockTailLog.ScrollToEnd ( );
      }
      else
      {
        if (!tabProperties.FileName.Equals (oldFileName))
        {
          textBlockTailLog.Clear ( );
          oldFileName = tabProperties.FileName;
        }

        if (tabProperties.FileEncoding == null)
        {
          if (!myReader.OpenTailFileStream (tabProperties.FileName))
          {
            MessageBox.Show (string.Format ("{0} '{1}'", Application.Current.FindResource ("FileNotFound"), tabProperties.File), string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
          }

          tabProperties.FileEncoding = myReader.FileEncoding;
        }
        else
        {
          if (!myReader.OpenTailFileStream (tabProperties.FileName, tabProperties.FileEncoding))
          {
            MessageBox.Show (string.Format ("{0} '{1}'", Application.Current.FindResource ("FileNotFound"), tabProperties.File), string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
          }
        }

        LogFile.APP_MAIN_WINDOW.StatusBarEncodeCB.SelectedValue = tabProperties.FileEncoding;

        if (myReader.FileExists (tabProperties.FileName))
        {
          childTabItem.Header = string.Format ("{0}", tabProperties.File);
          childTabItem.Foreground = Brushes.LimeGreen;

          WordWrap ( );
          
          if (!tailWorker.IsBusy)
            tailWorker.RunWorkerAsync ( );

          SetControlVisibility (true);
        }
        else
          MessageBox.Show (Application.Current.FindResource ("FileNotFound") as string, string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void btnStop_Click (object sender, RoutedEventArgs e)
    {
      tailWorker.CancelAsync ( );
    }

    private void btnPrint_Click (object sender, RoutedEventArgs e)
    {
      if (!(bool) checkBoxTimestamp.IsChecked)
        new PrintHelper (textBlockTailLog.LogEntries, tabProperties.File);
      else
        new PrintHelper (textBlockTailLog.LogEntries, tabProperties.File, true, StringFormatData.StringFormat);
    }

    private void btnOpenFile_Click (object sender, RoutedEventArgs e)
    {
      string fName = string.Empty;

      if (LogFile.OpenFileLogDialog (out fName))
        textBoxFileName.Text = fName;
    }

    private void comboBoxRefreshRate_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      if (!isInit)
        return;

      SettingsData.ETailRefreshRate selection = (SettingsData.ETailRefreshRate) comboBoxRefreshRate.SelectedItem;
      tabProperties.RefreshRate = selection;  
    }

    private void comboBoxThreadPriority_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      if (!isInit)
        return;

      ThreadPriority selection = (ThreadPriority) comboBoxThreadPriority.SelectedItem;
      tabProperties.ThreadPriority = selection;
    }

    private void btnShellOpen_Click (object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrEmpty (tabProperties.FileName))
      {
        ProcessStartInfo shellOpen = new ProcessStartInfo (tabProperties.FileName) { UseShellExecute = true };
        Process.Start (shellOpen);
      }
    }

    private void btnFileManager_Click (object sender, RoutedEventArgs e)
    {
      FileManager fileManager = new FileManager (SettingsData.EFileManagerState.OpenFileManager, tabProperties) { Owner = LogFile.APP_MAIN_WINDOW };
      fileManager.DoUpdate += fileManagerDoUpdate;
      fileManager.OpenFileAsNewTab += fileManagerGetProperties;
      fileManager.ShowDialog ( );
    }

    private void btnFileManagerAdd_Click (object sender, RoutedEventArgs e)
    {
      FileManager fileManager = new FileManager (SettingsData.EFileManagerState.AddFile, tabProperties) 
      { 
        Owner = LogFile.APP_MAIN_WINDOW,
        Title = string.Format ("FileManager - Add file '{0}'", tabProperties.File),
        Icon = new ImageSourceConverter ( ).ConvertFromString (@"pack://application:,,/Res/add.png") as ImageSource
      };

      fileManager.DoUpdate += fileManagerDoUpdate;
      fileManager.OpenFileAsNewTab += fileManagerGetProperties;
      fileManager.ShowDialog ( );
    }

    private void btnSettings_Click (object sender, RoutedEventArgs e)
    {
      Options settings = new Options ( ) { Owner = LogFile.APP_MAIN_WINDOW };
      settings.UpdateEvent += defaultPropertiesChanged;
      settings.ShowDialog ( );
    }

    private void btnClearTextBox_Click (object sender, RoutedEventArgs e)
    {
      textBlockTailLog.Clear ( );
    }

    private void btnFont_Click (object sender, RoutedEventArgs e)
    {
      System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;

      if (textBlockTailLog.FontStyle == FontStyles.Italic)
        fs = System.Drawing.FontStyle.Italic;
      if (textBlockTailLog.FontWeight == FontWeights.Bold)
        fs |= System.Drawing.FontStyle.Bold;

      System.Drawing.Font textBox = new System.Drawing.Font (textBlockTailLog.FontFamily.Source, (float) textBlockTailLog.FontSize, fs);
      System.Windows.Forms.FontDialog fontManager = new System.Windows.Forms.FontDialog ( ) { ShowEffects = false, Font = textBox, FontMustExist = true };

      if (fontManager.ShowDialog ( ) != System.Windows.Forms.DialogResult.Cancel)
      {
        tabProperties.FontType = fontManager.Font;
        textBlockTailLog.FontFamily = new FontFamily (tabProperties.FontType.Name);
        textBlockTailLog.FontSize = tabProperties.FontType.Size;
        textBlockTailLog.FontWeight = tabProperties.FontType.Bold ? FontWeights.Bold : FontWeights.Regular;
        textBlockTailLog.FontStyle = tabProperties.FontType.Italic ? FontStyles.Italic : FontStyles.Normal;
      }
    }

    private void checkBoxWordWrap_Click (object sender, RoutedEventArgs e)
    {
      WordWrap ( );
    }

    #endregion

    #region Thread

    void tailWorker_DoWork (object sender, DoWorkEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty (Thread.CurrentThread.Name))
          Thread.CurrentThread.Name = string.Format ("TailThread_{0}", childTabIndex);

        Thread.CurrentThread.Priority = tabProperties.ThreadPriority;

        childTabState = LogFile.STATUS_BAR_STATE_RUN;

        if (LogFile.APP_MAIN_WINDOW.StatusBarState.Dispatcher.CheckAccess ( ))
          LogFile.APP_MAIN_WINDOW.StatusBarState.Content = childTabState;
        else
          LogFile.APP_MAIN_WINDOW.StatusBarState.Dispatcher.Invoke (new Action (() => { LogFile.APP_MAIN_WINDOW.StatusBarState.Content = childTabState; }));

        if (SettingsHelper.TailSettings.ShowNLineAtStart == true)
        {
          tabProperties.LastRefreshTime = DateTime.Now;
          myReader.ReadLastNLines (SettingsHelper.TailSettings.LinesRead);

          string line = string.Empty;

          while ((line = myReader.TailStreamReader.ReadLine ( )) != null)
          {
            if (tabProperties.KillSpace)
            {
              if (!string.IsNullOrWhiteSpace (line))
                InvokeControlAccess (line);
            }
            else
              InvokeControlAccess (line);
          }
        }

        // start at the end of the file
        long lastMaxOffset = myReader.TailStreamReader.BaseStream.Length;

        while (tailWorker != null && !tailWorker.CancellationPending)
        {
          Thread.Sleep ((int) tabProperties.RefreshRate);

          // if the file size has not changed, idle
          if (myReader.TailStreamReader.BaseStream.Length == lastMaxOffset)
            continue;

          // file is suddenly empty
          if (myReader.TailStreamReader.BaseStream.Length < lastMaxOffset)
            lastMaxOffset = myReader.TailStreamReader.BaseStream.Length;

          // seek to the last max offset
          myReader.TailStreamReader.BaseStream.Seek (lastMaxOffset, SeekOrigin.Begin);

          // read out of the file until the EOF
          string line = string.Empty;

          while ((line = myReader.TailStreamReader.ReadLine ( )) != null)
          {
            if (tabProperties.KillSpace)
            {
              if (!string.IsNullOrWhiteSpace (line))
                InvokeControlAccess (line);
            }
            else
              InvokeControlAccess (line);

            tabProperties.LastRefreshTime = DateTime.Now;
          }

          // update the last max offset
          lastMaxOffset = myReader.TailStreamReader.BaseStream.Position;
        }

        if (tailWorker != null && tailWorker.CancellationPending == true)
          e.Cancel = true;
      }
      catch (Exception ex)
      {
        Debug.WriteLine (ex);
      }
    }

    void tailWorker_RunWorkerComplete (object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
        MessageBox.Show (e.Error.Message, string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
      else if (e.Cancelled)
      {
        childTabState = LogFile.STATUS_BAR_STATE_PAUSE;
        LogFile.APP_MAIN_WINDOW.StatusBarState.Content = childTabState;

        childTabItem.Header = tabProperties.File;
        childTabItem.Foreground = Brushes.Black;
        SetControlVisibility ( );

        if (stopThread)
          KillThread ( );
      }
    }

    public void StopThread ()
    {
      if (tailWorker.IsBusy)
      {
        tailWorker.CancelAsync ( );
        stopThread = true;
      }
    }

    private void KillThread ()
    {
      tailWorker.Dispose ( );
      tailWorker = null;
      myReader.CloseFileStream ( );
      myReader.Dispose ( );
    }

    #endregion

    #region HelpFunctions

    private void InitTailLog (int childTabIndex, TabItem tabItem)
    {
      tailWorker = new BackgroundWorker ( ) { WorkerSupportsCancellation = true };
      tailWorker.DoWork += tailWorker_DoWork;
      tailWorker.RunWorkerCompleted += tailWorker_RunWorkerComplete;

      this.childTabIndex = childTabIndex;
      childTabItem = tabItem;
      childTabState = LogFile.STATUS_BAR_STATE_PAUSE;

      myReader = new FileReader ( );

      ExtraIcons.DataContext = tabProperties;

      defaultPropertiesChanged (this, null);
      LoadHighlighting ( );
    }

    private void LoadHighlighting ()
    {
      // TODO Highlighting
    }

    /// <summary>
    /// Update file encoding property from MainWindow ComboBox (cbStsEncoding)
    /// </summary>
    /// <param name="encode">Encoding</param>
    public void UpdateFileEncoding (Encoding encode)
    {
      tabProperties.FileEncoding = encode;
    }

    private void WordWrap ( )
    {
      if (tabProperties.Wrap)
        textBlockTailLog.WordWrapping = true;
      else
        textBlockTailLog.WordWrapping = false;
    }

    private void InitComboBoxes ()
    {
      comboBoxRefreshRate.DataContext = LogFile.RefreshRate;
      comboBoxThreadPriority.DataContext = LogFile.ThreadPriority;
    }

    private void SetControlVisibility (bool visible = false)
    {
      if (visible)
      {
        btnStart.Visibility = System.Windows.Visibility.Hidden;
        btnStop.Visibility = System.Windows.Visibility.Visible;
        textBlockTailLog.TextEditorBackgroundColor = SettingsHelper.TailSettings.GuiDefaultBackgroundColor;
        textBlockTailLog.TextEditorForegroundColor = SettingsHelper.TailSettings.GuiDefaultForegroundColor;
        btnOpenFile.IsEnabled = !visible;
        textBoxFileName.IsReadOnly = visible;
      }
      else
      {
        btnStop.Visibility = System.Windows.Visibility.Hidden;
        btnStart.Visibility = System.Windows.Visibility.Visible;
        btnOpenFile.IsEnabled = !visible;
        textBoxFileName.IsReadOnly = visible;

        textBlockTailLog.TextEditorBackgroundColor = SettingsHelper.TailSettings.GuiDefaultInactiveBackgroundColor;
        textBlockTailLog.TextEditorForegroundColor = SettingsHelper.TailSettings.GuiDefaultInactiveForegroundColor;
      }
    }

    /// <summary>
    /// Get child state
    /// </summary>
    /// <returns>Pause or Record</returns>
    public string GetChildState ()
    {
      return (childTabState);
    }

    /// <summary>
    /// Get child tab index
    /// </summary>
    /// <returns>Index of tab</returns>
    public int GetChildTabIndex ()
    {
      return (childTabIndex);
    }

    /// <summary>
    /// Update Statusbar of parent window when change tabitem
    /// </summary>
    public void UpdateStatusBarOnTabSelectionChange ()
    {
      if (!string.IsNullOrEmpty (tabProperties.FileName) && myReader.FileEncoding != null)
      {
        LogFile.APP_MAIN_WINDOW.StatusBarEncoding.Dispatcher.Invoke (new Action (() =>
        {
          string time = tabProperties.LastRefreshTime.ToString (SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultTimeFormat));
          LogFile.APP_MAIN_WINDOW.StatusBarEncoding.Content = string.Format ("Size={0:0.###} Kb, Last refresh time={1}", myReader.FileSizeKB, time);
        }), DispatcherPriority.ApplicationIdle);

        LogFile.APP_MAIN_WINDOW.StatusBarLinesRead.Dispatcher.Invoke (new Action (() =>
        {
          LogFile.APP_MAIN_WINDOW.StatusBarLinesRead.Content = string.Format ("{0}{1}", Application.Current.FindResource ("LinesRead"), textBlockTailLog.LineCount);
        }), DispatcherPriority.ApplicationIdle);

        LogFile.APP_MAIN_WINDOW.StatusBarEncodeCB.Dispatcher.Invoke (new Action (() =>
        {
          LogFile.APP_MAIN_WINDOW.StatusBarEncodeCB.SelectedValue = tabProperties.FileEncoding;
        }), DispatcherPriority.ApplicationIdle);
      }
      else
      {
        LogFile.APP_MAIN_WINDOW.StatusBarEncoding.Dispatcher.Invoke (new Action (() =>
        {
          LogFile.APP_MAIN_WINDOW.StatusBarEncoding.Content = string.Empty;
        }), DispatcherPriority.ApplicationIdle);

        LogFile.APP_MAIN_WINDOW.StatusBarLinesRead.Dispatcher.Invoke (new Action (() =>
        {
          LogFile.APP_MAIN_WINDOW.StatusBarLinesRead.Content = string.Empty;
        }), DispatcherPriority.ApplicationIdle);
      }
    }

    private void InvokeControlAccess (string line)
    {
      if (!tabProperties.Timestamp)
      {
        textBlockTailLog.Dispatcher.Invoke (new Action (() =>
          {
            textBlockTailLog.AppendText (line);
          }), DispatcherPriority.ApplicationIdle);
      }
      else
      {
        textBlockTailLog.Dispatcher.Invoke (new Action (() =>
          {
            if (SettingsHelper.TailSettings.DefaultTimeFormat == SettingsData.ETimeFormat.HHMMd || SettingsHelper.TailSettings.DefaultTimeFormat == SettingsData.ETimeFormat.HHMMD)
              StringFormatData.StringFormat = string.Format ("{0} {1}:ss.fff", SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultDateFormat), SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultTimeFormat));
            if (SettingsHelper.TailSettings.DefaultTimeFormat == SettingsData.ETimeFormat.HHMMSSd || SettingsHelper.TailSettings.DefaultTimeFormat == SettingsData.ETimeFormat.HHMMSSD)
              StringFormatData.StringFormat = string.Format ("{0} {1}.fff", SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultDateFormat), SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultTimeFormat));

            textBlockTailLog.AppendText (line);
          }), DispatcherPriority.ApplicationIdle);
      }

      // Only when tab is active update statusbar!
      if (ActiveTab == true)
      {
        UpdateStatusBarOnTabSelectionChange ( );
      }
    }

    #endregion

    #region Events
    
    private void textBoxFileName_TextChanged (object sender, TextChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty (textBoxFileName.Text))
      {
        if (myReader.FileExists (textBoxFileName.Text))
        {
          tabProperties.FileName = textBoxFileName.Text;
          childTabItem.Header = tabProperties.File;
          btnFileManagerAdd.IsEnabled = true;
          btnShellOpen.IsEnabled = true;
          btnPrint.IsEnabled = true;
          ExtraIcons.IsEnabled = true;
        }
        else
        {
          childTabItem.Header = LogFile.TABBAR_CHILD_EMPTY_STRING;
          btnFileManagerAdd.IsEnabled = false;
          btnShellOpen.IsEnabled = false;
          btnPrint.IsEnabled = false;
          ExtraIcons.IsEnabled = false;
        }
      }
    }

    private void Page_Loaded (object sender, RoutedEventArgs e)
    {
      InitComboBoxes ( );
      isInit = true;

      comboBoxRefreshRate.SelectedValue = tabProperties.RefreshRate;
      comboBoxThreadPriority.SelectedValue = tabProperties.ThreadPriority;
    }

    private void defaultPropertiesChanged (object sender, EventArgs e)
    {
      if (tailWorker.IsBusy)
      {
        textBlockTailLog.TextEditorBackgroundColor = SettingsHelper.TailSettings.GuiDefaultBackgroundColor;
        textBlockTailLog.TextEditorForegroundColor = SettingsHelper.TailSettings.GuiDefaultForegroundColor;
      }
      else
      {
        textBlockTailLog.TextEditorBackgroundColor = SettingsHelper.TailSettings.GuiDefaultInactiveBackgroundColor;
        textBlockTailLog.TextEditorForegroundColor = SettingsHelper.TailSettings.GuiDefaultInactiveForegroundColor;
      }

      checkBoxOnTop.IsChecked = SettingsHelper.TailSettings.AlwaysOnTop;
      textBlockTailLog.ShowLineNumbers = SettingsHelper.TailSettings.ShowLineNumbers;
      textBlockTailLog.LineNumbersColor = SettingsHelper.TailSettings.GuiDefaultLineNumbersColor;
      textBlockTailLog.TextEditorSearchHighlightBackground = SettingsHelper.TailSettings.GuiDefaultHighlightBackgroundColor;
      textBlockTailLog.TextEditorSearchHighlightForeground = SettingsHelper.TailSettings.GuiDefaultHighlightForegroundColor;
      textBlockTailLog.AlwaysScrollIntoView = SettingsHelper.TailSettings.AlwaysScrollToEnd;
      textBlockTailLog.TextEditorSelectionColor = ((SolidColorBrush) (SettingsHelper.TailSettings.GuiDefaultHighlightColor)).Color;
    }

    private void fileManagerDoUpdate (object sender, EventArgs e)
    {
      MessageBox.Show ("FileManager do update");
    }

    private void fileManagerGetProperties (object sender, EventArgs e)
    {
      if (FileManagerDoOpenTab != null)
        FileManagerDoOpenTab (this, e);
    }

    #endregion
  }
}
