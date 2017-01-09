﻿using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Template.TextEditor.Converter;
using Org.Vs.TailForWin.Template.TextEditor.Data;
using Org.Vs.TailForWin.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for TailLog.xaml
  /// </summary>
  public partial class TailLog : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TailLog));

    private readonly TailLogData tabProperties;
    private BackgroundWorker tailWorker;
    private int childTabIndex;
    private TabItem childTabItem;
    private string childTabState;
    private string oldFileName = string.Empty;
    private FileReader myReader;
    private bool stopThread;
    private bool isInit;
    private MailClient mySmtp;

    /// <summary>
    /// filemanager save property
    /// </summary>
    private FileManagerData fileManagerProperties;

    /// <summary>
    /// new filters added, can save XML file
    /// </summary>
    private bool saveFilters;

    /// <summary>
    /// FileManager open new tab event handler
    /// </summary>
    public event EventHandler FileManagerDoOpenTab;

    /// <summary>
    /// Open the search box window event
    /// </summary>
    public event EventHandler ButtonSearchBox;

    /// <summary>
    /// new file added load default properties
    /// </summary>
    private event EventHandler NewFile;


    /// <summary>
    /// Releases all resources used by the TailLog.
    /// </summary>
    public void Dispose()
    {
      if(tailWorker != null)
      {
        tailWorker.Dispose();
        tailWorker = null;
      }

      myReader.Dispose();
      tabProperties.Dispose();
      mySmtp.Dispose();
    }

    /// <summary>
    /// Tab with properties from FileManager
    /// </summary>
    /// <param name="childTabIndex">Reference index</param>
    /// <param name="tabItem">Reference tab control parent</param>
    /// <param name="fileManagerProperties">Settings from FileManager</param>
    public TailLog(int childTabIndex, TabItem tabItem, FileManagerData fileManagerProperties)
    {
      InitializeComponent();

      this.fileManagerProperties = fileManagerProperties;

      tabProperties = new TailLogData
      {
        Wrap = fileManagerProperties.Wrap,
        KillSpace = fileManagerProperties.KillSpace,
        FileName = fileManagerProperties.FileName,
        FontType = fileManagerProperties.FontType,
        RefreshRate = fileManagerProperties.RefreshRate,
        ThreadPriority = fileManagerProperties.ThreadPriority,
        ListOfFilter = fileManagerProperties.ListOfFilter,
        Timestamp = fileManagerProperties.Timestamp,
        FileEncoding = fileManagerProperties.FileEncoding,
        OpenFromFileManager = fileManagerProperties.OpenFromFileManager,
        FilterState = fileManagerProperties.FilterState
      };

      InitTailLog(childTabIndex, tabItem);

      textBoxFileName.Text = tabProperties.FileName;
      SetToolTipDetailText();
      FilterState();
    }

    /// <summary>
    /// New tab with default settings
    /// </summary>
    /// <param name="childTabIndex">Reference index</param>
    /// <param name="tabItem">Reference tab control parent</param>
    public TailLog(int childTabIndex, TabItem tabItem)
    {
      InitializeComponent();

      System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;

      if(textBlockTailLog.TextEditorFontStyle == FontStyles.Italic)
        fs = System.Drawing.FontStyle.Italic;
      if(textBlockTailLog.TextEditorFontWeight == FontWeights.Bold)
        fs |= System.Drawing.FontStyle.Bold;

      System.Drawing.Font textBox = new System.Drawing.Font(textBlockTailLog.TextEditorFontFamily.Source, textBlockTailLog.TextEditorFontSize, fs);

      tabProperties = new TailLogData
      {
        Wrap = false,
        KillSpace = false,
        FontType = textBox,
        RefreshRate = SettingsHelper.TailSettings.DefaultRefreshRate,
        ThreadPriority = SettingsHelper.TailSettings.DefaultThreadPriority,
        FileEncoding = null
      };

      fileManagerProperties = new FileManagerData
      {
        ID = -1,
        FontType = tabProperties.FontType
      };

      InitTailLog(childTabIndex, tabItem);
    }

    private bool activeTab;

    /// <summary>
    /// Is tab active or not
    /// </summary>
    public bool ActiveTab
    {
      get
      {
        return (activeTab);
      }
      set
      {
        activeTab = value;
        textBlockTailLog.IsActiv = activeTab;
      }
    }

    /// <summary>
    /// FileManager properties
    /// </summary>
    public FileManagerData FileManagerProperties
    {
      get
      {
        return (fileManagerProperties);
      }
    }

    /// <summary>
    /// Update checkBoxOnTop.IsChecked
    /// </summary>
    /// <param name="isChecked">Topmost value from parent window</param>
    public void UpdateCheckBoxOnTopOnWindowTopmost(bool isChecked)
    {
      checkBoxOnTop.IsChecked = isChecked;
    }

    /// <summary>
    /// Find search text
    /// </summary>
    /// <param name="sd">Search data structure</param>
    public void FindNext(SearchData sd)
    {
      textBlockTailLog.FindNextItem(sd);
    }

    /// <summary>
    /// Get search count item
    /// </summary>
    public int SearchCount()
    {
      return (textBlockTailLog.SearchResultCount);
    }

    /// <summary>
    /// Searchbox is shown
    /// </summary>
    public void SearchBoxActive()
    {
      textBlockTailLog.IsSearching = true;
    }

    /// <summary>
    /// Searchbox is hidden
    /// </summary>
    public void SearchBoxInactive()
    {
      textBlockTailLog.RemoveSearchHighlight();
    }

    /// <summary>
    /// Find text changed
    /// </summary>
    public void FindWhatTextChanged()
    {
      textBlockTailLog.FindWhatTextChanged();
    }

    /// <summary>
    /// Wrap around while searching
    /// </summary>
    /// <param name="wrap">true or false</param>
    public void WrapAround(bool wrap)
    {
      textBlockTailLog.WrapAround = wrap;
    }

    /// <summary>
    /// Bookmark lines
    /// </summary>
    /// <param name="bookmark">true or false</param>
    public void BookmarkLine(bool bookmark)
    {
      textBlockTailLog.BookmarkLine = bookmark;
    }

    /// <summary>
    /// Go to line number
    /// </summary>
    public void GoToLineNumber()
    {
      GoToLine goToLineDialog = new GoToLine(GetMinLineNumber(), GetMaxLineNumber())
      {
        Owner = LogFile.APP_MAIN_WINDOW
      };
      goToLineDialog.GoToLineNumber += GoToLineNumberEvent;

      goToLineDialog.ShowDialog();
    }

    /// <summary>
    /// Is textbox filename focued
    /// </summary>
    public bool TextBoxFileNameIsFocused
    {
      get
      {
        return (textBoxFileName.IsFocused);
      }
    }

    /// <summary>
    /// Toggle always on top on/off
    /// </summary>
    public void AlwaysOnTop()
    {
      checkBoxOnTop.IsChecked = checkBoxOnTop.IsChecked != true;

      checkBoxOnTop_Click(checkBoxOnTop, null);
    }

    /// <summary>
    /// Toggle filter on/off
    /// </summary>
    public void FilterOnOff()
    {
      tabProperties.FilterState = checkBoxFilter.IsChecked != true;

      FilterState();
    }

    /// <summary>
    /// Is thread busy
    /// </summary>
    public bool IsThreadBusy
    {
      get;
      private set;
    }

    #region ClickEvent

    private void checkBoxOnTop_Click(object sender, RoutedEventArgs e)
    {
      LogFile.APP_MAIN_WINDOW.MainWindowTopmost = checkBoxOnTop.IsChecked == true;
    }

    /// <summary>
    /// Open search dialog
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    public void btnSearch_Click(object sender, RoutedEventArgs e)
    {
      if(tabProperties.File == null)
        return;
      if(textBlockTailLog.LineCount == 0)
        return;
      if(ButtonSearchBox == null)
        return;

      FileManagerData data = new FileManagerData
      {
        File = tabProperties.File,
        FileName = tabProperties.FileName
      };

      FileManagerDataEventArgs eventData = new FileManagerDataEventArgs(data);
      ButtonSearchBox(this, eventData);
      eventData.Dispose();
    }

    /// <summary>
    /// Start tail process
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    public void btnStart_Click(object sender, RoutedEventArgs e)
    {
      if(string.IsNullOrEmpty(tabProperties.FileName))
      {
        LOG.Info("{0} file not found", System.Reflection.MethodBase.GetCurrentMethod().Name);

        textBlockTailLog.AppendText(string.Format("{0} - {1}", DateTime.Now, Application.Current.FindResource("NoFilenameEntered")));
        textBlockTailLog.ScrollToEnd();
        textBlockTailLog.FileNameAvailable = false;
      }
      else
      {
        if(!tabProperties.FileName.Equals(oldFileName))
          textBlockTailLog.Clear();

        if(tabProperties.FileEncoding == null)
        {
          if(!myReader.OpenTailFileStream(tabProperties.FileName))
          {
            LOG.Info("{0} file not found '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, tabProperties.FileName);
            MessageBox.Show(string.Format("{0} '{1}'", Application.Current.FindResource("FileNotFound"), tabProperties.File), string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
          }

          tabProperties.FileEncoding = myReader.FileEncoding;
        }
        else
        {
          if(!myReader.OpenTailFileStream(tabProperties.FileName, tabProperties.FileEncoding))
          {
            LOG.Info("{0} file not found '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, tabProperties.FileName);
            MessageBox.Show(string.Format("{0} '{1}'", Application.Current.FindResource("FileNotFound"), tabProperties.File), string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
          }
        }

        LogFile.APP_MAIN_WINDOW.StatusBarEncodeCb.SelectedValue = tabProperties.FileEncoding;

        if(FileReader.FileExists(tabProperties.FileName))
        {
          if(string.IsNullOrEmpty(textBoxFileName.Text))
            textBoxFileName.Text = tabProperties.FileName;

          childTabItem.Header = string.Format("{0}", tabProperties.File);
          childTabItem.Style = (Style) FindResource("TabItemTailStyle");
          textBlockTailLog.FileNameAvailable = true;

          WordWrap();

          if(!tailWorker.IsBusy)
          {
            LOG.Info("{0} start tail file '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, tabProperties.FileName);

            tailWorker.RunWorkerAsync();
            btnStop.IsEnabled = true;
          }

          SetControlVisibility(true);
        }
        else
        {
          LOG.Info("{0} file not found '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, tabProperties.FileName);
          MessageBox.Show(Application.Current.FindResource("FileNotFound") as string, string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }

    /// <summary>
    /// Pause tail process
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    public void btnStop_Click(object sender, RoutedEventArgs e)
    {
      oldFileName = tabProperties.FileName;
      btnStop.IsEnabled = false;
      tailWorker.CancelAsync();
    }

    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
      if(checkBoxTimestamp.IsChecked != null && !(bool) checkBoxTimestamp.IsChecked)
        new PrintHelper(textBlockTailLog.LogEntries, tabProperties.File);
      else
        new PrintHelper(textBlockTailLog.LogEntries, tabProperties.File, true, StringFormatData.StringFormat);
    }

    /// <summary>
    /// Open file dialog
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    public void btnOpenFile_Click(object sender, RoutedEventArgs e)
    {
      string fName;

      if(!LogFile.OpenFileLogDialog(out fName, "Logfiles (*.log)|*.log|Textfiles (*.txt)|*.txt|All files (*.*)|*.*", Application.Current.FindResource("OpenFileDialog") as string))
        return;

      NewFile?.Invoke(this, EventArgs.Empty);

      textBoxFileName.Text = fName;
    }

    /// <summary>
    /// Open file from parameter
    /// </summary>
    /// <param name="fName">File with full path</param>
    public void OpenFileFromParameter(string fName)
    {
      NewFile?.Invoke(this, EventArgs.Empty);

      textBoxFileName.Text = fName;
    }

    private void comboBoxRefreshRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!isInit)
        return;

      ETailRefreshRate selection = (ETailRefreshRate) comboBoxRefreshRate.SelectedItem;
      tabProperties.RefreshRate = selection;
      e.Handled = true;
    }

    private void comboBoxThreadPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!isInit)
        return;

      ThreadPriority selection = (ThreadPriority) comboBoxThreadPriority.SelectedItem;
      tabProperties.ThreadPriority = selection;
      e.Handled = true;
    }

    private void btnShellOpen_Click(object sender, RoutedEventArgs e)
    {
      if(!string.IsNullOrEmpty(tabProperties.FileName))
      {
        ProcessStartInfo shellOpen = new ProcessStartInfo(tabProperties.FileName)
        {
          UseShellExecute = true
        };
        Process.Start(shellOpen);
      }
    }

    /// <summary>
    /// Open file manager dialog
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    public void btnFileManager_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        FileManager fileManager = new FileManager(EFileManagerState.OpenFileManager, tabProperties)
        {
          Owner = LogFile.APP_MAIN_WINDOW
        };
        fileManager.DoUpdate += FileManagerDoUpdate;
        fileManager.OpenFileAsNewTab += FileManagerGetProperties;
        fileManager.ShowDialog();
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void btnFileManagerAdd_Click(object sender, RoutedEventArgs e)
    {
      FileManager fileManager = new FileManager(EFileManagerState.AddFile, tabProperties)
      {
        Owner = LogFile.APP_MAIN_WINDOW,
        Title = string.Format("FileManager - Add file '{0}'", tabProperties.File),
        Icon = new ImageSourceConverter().ConvertFromString(@"pack://application:,,/Res/add.ico") as ImageSource
      };

      fileManager.DoUpdate += FileManagerDoUpdate;
      fileManager.OpenFileAsNewTab += FileManagerGetProperties;
      fileManager.ShowDialog();
    }

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
      Options settings = new Options
      {
        Owner = LogFile.APP_MAIN_WINDOW
      };

      settings.UpdateEvent += DefaultPropertiesChanged;
      settings.ShowDialog();
    }

    /// <summary>
    /// Clears all content in TextEditor
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    public void btnClearTextBox_Click(object sender, RoutedEventArgs e)
    {
      textBlockTailLog.Clear();
    }

    private void btnFont_Click(object sender, RoutedEventArgs e)
    {
      System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;

      if(textBlockTailLog.FontStyle == FontStyles.Italic)
        fs = System.Drawing.FontStyle.Italic;
      if(textBlockTailLog.FontWeight == FontWeights.Bold)
        fs |= System.Drawing.FontStyle.Bold;

      System.Drawing.Font textBox = new System.Drawing.Font(textBlockTailLog.TextEditorFontFamily.Source, textBlockTailLog.TextEditorFontSize, fs);
      System.Windows.Forms.FontDialog fontManager = new System.Windows.Forms.FontDialog
      {
        ShowEffects = false,
        Font = textBox,
        FontMustExist = true
      };

      if(fontManager.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      tabProperties.FontType = fontManager.Font;
      SetFontInTextEditor();
    }

    private void checkBoxWordWrap_Click(object sender, RoutedEventArgs e)
    {
      WordWrap();
    }

    private void btnFilters_Click(object sender, RoutedEventArgs e)
    {
      Filters filters = new Filters(tabProperties)
      {
        Owner = LogFile.APP_MAIN_WINDOW
      };

      filters.SaveNow += FilterTrigger;
      filters.ShowDialog();

      if(!saveFilters)
        return;

      FileManagerStructure fms = new FileManagerStructure();

      if(fileManagerProperties.FontType == null)
        fileManagerProperties.FontType = tabProperties.FontType;

      fms.UpdateNode(fileManagerProperties);
      textBlockTailLog.UpdateFilters(tabProperties.ListOfFilter);
      saveFilters = false;
    }

    private void checkBoxFilter_Click(object sender, RoutedEventArgs e)
    {
      if(checkBoxFilter.IsChecked != null)
        tabProperties.FilterState = (bool) checkBoxFilter.IsChecked;

      FilterState();
    }

    #endregion

    #region Thread

    private void tailWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        if(string.IsNullOrEmpty(Thread.CurrentThread.Name))
          Thread.CurrentThread.Name = string.Format("TailThread_{0}", childTabIndex);

        Thread.CurrentThread.Priority = tabProperties.ThreadPriority;

        childTabState = LogFile.STATUS_BAR_STATE_RUN;
        IsThreadBusy = true;

        LogFile.APP_MAIN_WINDOW.Dispatcher.Invoke(new Action(() =>
        {
          LogFile.APP_MAIN_WINDOW.SetSbIconText();
        }));

        if(LogFile.APP_MAIN_WINDOW.StatusBarState.Dispatcher.CheckAccess())
          LogFile.APP_MAIN_WINDOW.StatusBarState.Content = childTabState;
        else
          LogFile.APP_MAIN_WINDOW.StatusBarState.Dispatcher.Invoke(new Action(() =>
        {
          LogFile.APP_MAIN_WINDOW.StatusBarState.Content = childTabState;
        }));

        if(SettingsHelper.TailSettings.ShowNLineAtStart)
        {
          tabProperties.LastRefreshTime = DateTime.Now;
          myReader.ReadLastNLines(SettingsHelper.TailSettings.LinesRead);

          string line;

          while((line = myReader.TailStreamReader.ReadLine()) != null)
          {
            if(tabProperties.KillSpace)
            {
              if(!string.IsNullOrWhiteSpace(line))
                InvokeControlAccess(line);
            }
            else
              InvokeControlAccess(line);
          }
        }

        // start at the end of the file
        long lastMaxOffset = myReader.TailStreamReader.BaseStream.Length;

        while(tailWorker != null && !tailWorker.CancellationPending)
        {
          Thread.Sleep((int) tabProperties.RefreshRate);

          if(myReader.TailStreamReader == null)
            break;

          // if the file size has not changed, idle
          if(myReader.TailStreamReader.BaseStream.Length == lastMaxOffset)
            continue;

          // file is suddenly empty
          if(myReader.TailStreamReader.BaseStream.Length < lastMaxOffset)
            lastMaxOffset = myReader.TailStreamReader.BaseStream.Length;

          // seek to the last max offset
          myReader.TailStreamReader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

          // read out of file until EOF
          string line;

          while((line = myReader.TailStreamReader.ReadLine()) != null)
          {
            if(tabProperties.KillSpace)
            {
              if(!string.IsNullOrWhiteSpace(line))
                InvokeControlAccess(line);
            }
            else
              InvokeControlAccess(line);

            tabProperties.LastRefreshTime = DateTime.Now;
          }

          // update the last max offset
          lastMaxOffset = myReader.TailStreamReader.BaseStream.Position;
        }

        if(tailWorker == null || !tailWorker.CancellationPending)
          return;

        myReader.CloseFileStream();

        e.Cancel = true;
      }
      catch(ObjectDisposedException ex)
      {
        MessageBox.Show(Application.Current.FindResource("FileObjectDisposedException").ToString(), string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);

        StopThread();
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void tailWorker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
    {
      if(e.Error != null)
      {
        LOG.Info("{0} can not tail '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, e.Error.Message);
        MessageBox.Show(e.Error.Message, string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else if(e.Cancelled)
      {
        childTabState = LogFile.STATUS_BAR_STATE_PAUSE;
        LogFile.APP_MAIN_WINDOW.StatusBarState.Content = childTabState;
        childTabItem.Header = tabProperties.File;
        childTabItem.Style = (Style) FindResource("TabItemStopStyle");
        SetControlVisibility();
        SetToolTipDetailText();
        IsThreadBusy = false;

        LogFile.APP_MAIN_WINDOW.Dispatcher.Invoke(new Action(() =>
        {
          LogFile.APP_MAIN_WINDOW.SetSbIconText();
        }));

        if(stopThread)
          KillThread();
      }
    }

    /// <summary>
    /// Stops tail thread
    /// </summary>
    public void StopThread()
    {
      if(!tailWorker.IsBusy)
        return;

      tailWorker.CancelAsync();
      stopThread = true;
    }

    private void KillThread()
    {
      tailWorker.Dispose();
      tailWorker = null;
      myReader.CloseFileStream();
      myReader.Dispose();
    }

    #endregion

    #region HelpFunctions

    private void InitTailLog(int chldTabIdx, TabItem tabItem)
    {
      tailWorker = new BackgroundWorker
      {
        WorkerSupportsCancellation = true
      };
      tailWorker.DoWork += tailWorker_DoWork;
      tailWorker.RunWorkerCompleted += tailWorker_RunWorkerComplete;

      SetFontInTextEditor();

      childTabIndex = chldTabIdx;
      childTabItem = tabItem;
      childTabState = LogFile.STATUS_BAR_STATE_PAUSE;

      myReader = new FileReader();
      mySmtp = new MailClient();
      mySmtp.InitClient();

      ExtraIcons.DataContext = tabProperties;

      DefaultPropertiesChanged(this, null);

      textBlockTailLog.Alert += AlertTrigger;
      NewFile += NewFileOpend;
    }

    private void SetFontInTextEditor()
    {
      textBlockTailLog.TextEditorFontFamily = new FontFamily(tabProperties.FontType.Name);
      textBlockTailLog.TextEditorFontSize = (int) tabProperties.FontType.Size;
      textBlockTailLog.TextEditorFontWeight = tabProperties.FontType.Bold ? FontWeights.Bold : FontWeights.Regular;
      textBlockTailLog.TextEditorFontStyle = tabProperties.FontType.Italic ? FontStyles.Italic : FontStyles.Normal;
    }

    /// <summary>
    /// Update file encoding property from MainWindow ComboBox (cbStsEncoding)
    /// </summary>
    /// <param name="encode">Encoding</param>
    public void UpdateFileEncoding(Encoding encode)
    {
      tabProperties.FileEncoding = encode;
    }

    private void WordWrap()
    {
      textBlockTailLog.WordWrapping = tabProperties.Wrap;
    }

    private void FilterState()
    {
      if(tabProperties.FilterState)
      {
        checkBoxFilter.IsChecked = tabProperties.FilterState;
        textBlockTailLog.FilterOn = tabProperties.FilterState;
        textBlockTailLog.UpdateFilters(tabProperties.ListOfFilter);
      }
      else
      {
        checkBoxFilter.IsChecked = tabProperties.FilterState;
        textBlockTailLog.FilterOn = tabProperties.FilterState;
      }

      FileManagerStructure fms = new FileManagerStructure();
      fileManagerProperties.FilterState = tabProperties.FilterState;
      fms.UpdateNode(fileManagerProperties);
    }

    private void InitComboBoxes()
    {
      comboBoxRefreshRate.DataContext = LogFile.RefreshRate;
      comboBoxThreadPriority.DataContext = LogFile.ThreadPriority;
    }

    private void SetControlVisibility(bool visible = false)
    {
      if(visible)
      {
        btnStart.Visibility = Visibility.Hidden;
        btnStop.Visibility = Visibility.Visible;
        textBlockTailLog.TextEditorBackgroundColor = SettingsHelper.TailSettings.GuiDefaultBackgroundColor;
        textBlockTailLog.TextEditorForegroundColor = SettingsHelper.TailSettings.GuiDefaultForegroundColor;
        btnOpenFile.IsEnabled = !visible;
        textBoxFileName.IsReadOnly = visible;
      }
      else
      {
        btnStop.Visibility = Visibility.Hidden;
        btnStart.Visibility = Visibility.Visible;
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
    public string GetChildState()
    {
      return (childTabState);
    }

    /// <summary>
    /// Get child tab index
    /// </summary>
    /// <returns>Index of tab</returns>
    public int GetChildTabIndex()
    {
      return (childTabIndex);
    }

    /// <summary>
    /// Update Statusbar of parent window when change tabitem
    /// </summary>
    public void UpdateStatusBarOnTabSelectionChange()
    {
      if(!string.IsNullOrEmpty(tabProperties.FileName) && myReader.FileEncoding != null)
      {
        LogFile.APP_MAIN_WINDOW.Dispatcher.Invoke(new Action(() =>
      {
        string time = tabProperties.LastRefreshTime.ToString(SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultTimeFormat));
        LogFile.APP_MAIN_WINDOW.StatusBarEncoding.Content = string.Format("Size={0:0.###} Kb, Last refresh time={1}", myReader.FileSizeKB, time);
        LogFile.APP_MAIN_WINDOW.StatusBarLinesRead.Content = string.Format("{0}{1}", Application.Current.FindResource("LinesRead"), textBlockTailLog.LineCount);
        LogFile.APP_MAIN_WINDOW.StatusBarEncodeCb.SelectedValue = tabProperties.FileEncoding;

        SetToolTipDetailText();
      }), DispatcherPriority.Background);
      }
      else
      {
        LogFile.APP_MAIN_WINDOW.Dispatcher.Invoke(new Action(() =>
      {
        LogFile.APP_MAIN_WINDOW.StatusBarEncoding.Content = string.Empty;
        LogFile.APP_MAIN_WINDOW.StatusBarLinesRead.Content = string.Empty;

        SetToolTipDetailText();
      }), DispatcherPriority.Background);
      }
    }

    private void InvokeControlAccess(string line)
    {
      if(!tabProperties.Timestamp)
      {
        textBlockTailLog.Dispatcher.Invoke(new Action(() => textBlockTailLog.AppendText(line)), DispatcherPriority.ApplicationIdle);
      }
      else
      {
        textBlockTailLog.Dispatcher.Invoke(new Action(() =>
      {
        if(SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMd || SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMD)
          StringFormatData.StringFormat = string.Format("{0} {1}:ss.fff", SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultDateFormat), SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultTimeFormat));
        if(SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMSSd || SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMSSD)
          StringFormatData.StringFormat = string.Format("{0} {1}.fff", SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultDateFormat), SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultTimeFormat));

        textBlockTailLog.AppendText(line);
      }), DispatcherPriority.Background);
      }

      // Only when tab is active update statusbar!
      if(ActiveTab)
        UpdateStatusBarOnTabSelectionChange();
    }

    /// <summary>
    /// Get min linenumber
    /// </summary>
    /// <returns>min linenumber</returns>
    private int GetMinLineNumber()
    {
      if(textBlockTailLog.LogEntries.Count > 0)
        return (textBlockTailLog.LogEntries[0].Index);

      return (-1);
    }

    /// <summary>
    /// Get max linenumber
    /// </summary>
    /// <returns>max linenumber</returns>
    private int GetMaxLineNumber()
    {
      if(textBlockTailLog.LogEntries.Count > 0)
        return (textBlockTailLog.LogEntries[textBlockTailLog.LogEntries.Count - 1].Index);

      return (-1);
    }

    private void SetToolTipDetailText()
    {
      if(string.IsNullOrEmpty(tabProperties.File))
      {
        LogFile.APP_MAIN_WINDOW.ToolTipDetailText = string.Empty;
        return;
      }

      LogFile.APP_MAIN_WINDOW.ToolTipDetailText = string.Format(tailWorker.IsBusy ? "Tailing {0}" : "Pause {0}", tabProperties.File);
    }

    #endregion

    #region Events

    private void GoToLineNumberEvent(object sender, EventArgs e)
    {
      if(e.GetType() != typeof(GoToLineData))
        return;

      var goToLineData = e as GoToLineData;

      if(goToLineData != null)
        textBlockTailLog.GoToLineNumber(goToLineData.LineNumber);
    }

    private void NewFileOpend(object sender, EventArgs e)
    {
      if(fileManagerProperties != null)
        fileManagerProperties = new FileManagerData
        {
          ID = -1
        };

      System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;

      if(textBlockTailLog.TextEditorFontStyle == FontStyles.Italic)
        fs = System.Drawing.FontStyle.Italic;
      if(textBlockTailLog.TextEditorFontWeight == FontWeights.Bold)
        fs |= System.Drawing.FontStyle.Bold;

      System.Drawing.Font textBox = new System.Drawing.Font(textBlockTailLog.FontFamily.Source, (float) textBlockTailLog.FontSize, fs);

      tabProperties.Wrap = false;
      tabProperties.KillSpace = false;
      tabProperties.FontType = textBox;
      tabProperties.ThreadPriority = SettingsHelper.TailSettings.DefaultThreadPriority;
      tabProperties.RefreshRate = SettingsHelper.TailSettings.DefaultRefreshRate;
      tabProperties.FileEncoding = null;

      comboBoxRefreshRate.SelectedItem = tabProperties.RefreshRate;
      comboBoxThreadPriority.SelectedItem = tabProperties.ThreadPriority;

      checkBoxFilter.IsChecked = false;
      saveFilters = false;
    }

    private void textBoxFileName_TextChanged(object sender, TextChangedEventArgs e)
    {
      if(string.IsNullOrEmpty(textBoxFileName.Text))
        return;

      if(FileReader.FileExists(textBoxFileName.Text))
      {
        tabProperties.FileName = textBoxFileName.Text;
        childTabItem.Header = tabProperties.File;

        if(!fileManagerProperties.OpenFromFileManager)
          btnFileManagerAdd.IsEnabled = true;

        btnShellOpen.IsEnabled = true;
        btnPrint.IsEnabled = true;
        ExtraIcons.IsEnabled = true;

        SetToolTipDetailText();
      }
      else
      {
        childTabItem.Header = LogFile.TABBAR_CHILD_EMPTY_STRING;
        btnFileManagerAdd.IsEnabled = false;
        btnShellOpen.IsEnabled = false;
        btnPrint.IsEnabled = false;
        ExtraIcons.IsEnabled = false;

        tabProperties.ListOfFilter.Clear();
        checkBoxFilter.IsChecked = false;
        saveFilters = false;
      }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      InitComboBoxes();
      isInit = true;

      comboBoxRefreshRate.SelectedValue = tabProperties.RefreshRate;
      comboBoxThreadPriority.SelectedValue = tabProperties.ThreadPriority;
    }

    private void DefaultPropertiesChanged(object sender, EventArgs e)
    {
      if(tailWorker.IsBusy)
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

      SoundPlay.InitSoundPlay(SettingsHelper.TailSettings.AlertSettings.SoundFileNameFullPath);

      if(SettingsHelper.TailSettings.AlertSettings.SendEMail)
        mySmtp.InitClient();
    }

    private static void FileManagerDoUpdate(object sender, EventArgs e)
    {
#if DEBUG
      MessageBox.Show("FileManager do update");
#endif
    }

    private void FileManagerGetProperties(object sender, EventArgs e)
    {
      if(FileManagerDoOpenTab != null)
        FileManagerDoOpenTab(this, e);
    }

    public void DragEnterHelper(object sender, DragEventArgs e)
    {
      e.Handled = !tailWorker.IsBusy;

      if(e.Source == sender)
        e.Effects = DragDropEffects.None;
    }

    public void DropHelper(object sender, DragEventArgs e)
    {
      if(tailWorker.IsBusy)
      {
        MessageBox.Show(Application.Current.FindResource("DragDropRunningWarining") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
        e.Handled = false;
        return;
      }

      e.Handled = true;

      try
      {
        var text = e.Data.GetData(DataFormats.FileDrop);

        if(text == null)
          return;

        string fileName = string.Format("{0}", ((string[]) text)[0]);

        if(NewFile != null)
          NewFile(this, EventArgs.Empty);

        textBoxFileName.Text = fileName;
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void textBoxFileName_PreviewDragOver(object sender, DragEventArgs e)
    {
      e.Handled = !tailWorker.IsBusy;
    }

    private void AlertTrigger(object sender, EventArgs e)
    {
      LogEntry alertTriggerData = new LogEntry();

      if(e.GetType() == typeof(AlertTriggerEventArgs))
      {
        var alertTriggerEventArgs = e as AlertTriggerEventArgs;

        if(alertTriggerEventArgs != null)
          alertTriggerData = alertTriggerEventArgs.GetData();
      }

      if(SettingsHelper.TailSettings.AlertSettings.BringToFront)
        LogFile.BringMainWindowToFront();

      if(SettingsHelper.TailSettings.AlertSettings.PopupWnd)
      {
        var alertPopUp = new FancyPopUp
        {
          PopUpAlert = tabProperties.File,
          PopUpAlertDetail = alertTriggerData.Message
        };
        LogFile.APP_MAIN_WINDOW.MainWndTaskBarIcon.ShowCustomBalloon(alertPopUp, System.Windows.Controls.Primitives.PopupAnimation.Slide, 7000);
      }

      if(SettingsHelper.TailSettings.AlertSettings.PlaySoundFile)
        SoundPlay.Play(false);

      if(!SettingsHelper.TailSettings.AlertSettings.SendEMail)
        return;
      if(!mySmtp.InitSucces)
        return;

      string message = string.Format("{0}\t{1}", alertTriggerData.Index, alertTriggerData.Message);
      mySmtp.SendMail("AlertTrigger", message);
    }

    private void FilterTrigger(object sender, EventArgs e)
    {
      if(fileManagerProperties != null)
        saveFilters = true;
    }

    #endregion
  }
}
