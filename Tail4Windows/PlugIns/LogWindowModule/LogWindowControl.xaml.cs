using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.Controllers;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels;
using Org.Vs.TailForWin.PlugIns.FontChooserModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Controller;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.QuickAddModule;
using Org.Vs.TailForWin.PlugIns.QuickAddModule.ViewModels;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for LogWindowControl.xaml
  /// </summary>
  public partial class LogWindowControl : ILogWindowControl, IFileDragDropTarget
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogWindowControl));
    private static readonly object LogWindowControlLock = new object();

    private CancellationTokenSource _cts;
    private readonly PrintController _printerController;
    private readonly IXmlSearchHistory<QueueSet<string>> _historyController;
    private readonly IXmlFileManager _xmlFileManagerController;
    private QueueSet<string> _historyQueueSet;

    #region Events

    /// <summary>
    /// On lines and time changed event
    /// </summary>
    public event LinesRefreshTimeChangedEventHandler OnLinesTimeChanged;

    /// <summary>
    /// On status changed event
    /// </summary>
    public event StatusChangedEventHandler OnStatusChanged;

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogWindowControl()
    {
      InitializeComponent();

      DataContext = this;
      _printerController = new PrintController();
      _historyController = new XmlHistoryController();
      _xmlFileManagerController = new XmlFileManagerController();

      TailReader = new LogReadService();
      CurrentTailData = new TailData();

      ((AsyncCommand<object>) StartTailCommand).PropertyChanged += SaveHistoryCompleted;
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedCompleted;
    }

    /// <summary>
    /// <see cref="ILogReadService"/>
    /// </summary>
    public ILogReadService TailReader
    {
      get;
      private set;
    }

    private DragSupportTabItem _logWindowTabItem;

    /// <summary>
    /// LogWindowTabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem LogWindowTabItem
    {
      get => _logWindowTabItem;
      set
      {
        _logWindowTabItem = value;
        _logWindowTabItem.TabHeaderBackgroundChanged -= TabItemTabHeaderBackgroundChanged;
        _logWindowTabItem.TabHeaderBackgroundChanged += TabItemTabHeaderBackgroundChanged;
      }
    }

    private EStatusbarState _logWindowState;

    /// <summary>
    /// Current LogWindowState <see cref="EStatusbarState"/>
    /// </summary>
    public EStatusbarState LogWindowState
    {
      get => _logWindowState;
      set
      {
        _logWindowState = value;

        if ( IsSelected )
          OnStatusChanged?.Invoke(this, new StatusChangedArgs(LogWindowState, CurrentTailData.FileEncoding, LinesRead, TailReader.SizeRefreshTime));

        OnPropertyChanged();
        OnPropertyChanged(nameof(ThreadPriorityIsEnable));
      }
    }

    /// <summary>
    /// Is selected
    /// </summary>
    public bool IsSelected => LogWindowTabItem != null && LogWindowTabItem.IsSelected;

    private TailData _currentTailData;

    /// <summary>
    /// Current tail data <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _currentTailData;
      set
      {
        _currentTailData = value;
        OnPropertyChanged();
      }
    }

    private bool _fileIsValid;

    /// <summary>
    /// Current file is valid
    /// </summary>
    public bool FileIsValid
    {
      get => _fileIsValid;
      set
      {
        if ( value == _fileIsValid )
          return;

        _fileIsValid = value;
        OnPropertyChanged();
      }
    }

    private bool _logFileComboBoxHasFocus;

    /// <summary>
    /// LogFileComboBox has focus
    /// </summary>
    public bool LogFileComboBoxHasFocus
    {
      get => _logFileComboBoxHasFocus;
      set
      {
        _logFileComboBoxHasFocus = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Current LogFile history
    /// </summary>
    public ObservableCollection<string> LogFileHistory
    {
      get;
      set;
    } = new ObservableCollection<string>();

    private string _selectedItem;

    /// <summary>
    /// Selected item
    /// </summary>
    public string SelectedItem
    {
      get => _selectedItem;
      set
      {
        // Strange workaround! Sometimes SelectedItem will set twice and value is null - no clue why!
        if ( string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(CurrentTailData.FileName) )
          value = CurrentTailData.FileName;

        _selectedItem = value;
        OnPropertyChanged();

        SetCurrentLogFileName();
      }
    }

    private double _splitterPosition;

    /// <summary>
    /// Splitter height
    /// </summary>
    public double SplitterPosition
    {
      get => _splitterPosition;
      set
      {
        if ( Equals(value, _splitterPosition) )
          return;

        _splitterPosition = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Lines read
    /// </summary>
    public int LinesRead => SplitWindow.LinesRead;

    /// <summary>
    /// SplitWindow control
    /// </summary>
    public ISplitWindowControl SplitWindow => SplitWindowControl;

    /// <summary>
    /// Thread priority is enable
    /// </summary>
    public bool ThreadPriorityIsEnable => LogWindowState != EStatusbarState.Busy;

    #region Commands

    private ICommand _openFileCommand;

    /// <summary>
    /// Open file command
    /// </summary>
    public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(p => LogWindowState != EStatusbarState.Busy, p => ExecuteOpenFileCommand()));

    private IAsyncCommand _startTailCommand;

    /// <summary>
    /// Start tail command
    /// </summary>
    public IAsyncCommand StartTailCommand => _startTailCommand ?? (_startTailCommand = AsyncCommand.Create(p => FileIsValid && LogWindowState != EStatusbarState.Busy, ExecuteStartTailCommandAsync));

    private ICommand _stopTailCommand;

    /// <summary>
    /// Stop tail command
    /// </summary>
    public ICommand StopTailCommand => _stopTailCommand ?? (_stopTailCommand = new RelayCommand(p => LogWindowState == EStatusbarState.Busy, p => ExecuteStopTailCommand()));

    private ICommand _addToFileManagerCommand;

    /// <summary>
    /// Add to FileManager command
    /// </summary>
    public ICommand AddToFileManagerCommand => _addToFileManagerCommand ?? (_addToFileManagerCommand = new RelayCommand(p => FileIsValid && CurrentTailData != null && !CurrentTailData.OpenFromFileManager, p => ExecuteAddToFileManagerCommand()));

    private ICommand _openFileManagerCommand;

    /// <summary>
    /// Open FileManager command
    /// </summary>
    public ICommand OpenFileManagerCommand => _openFileManagerCommand ?? (_openFileManagerCommand = new RelayCommand(p => ExecuteOpenFileManagerCommand()));

    private ICommand _previewDragEnterCommand;

    /// <summary>
    /// Preview drag enter command
    /// </summary>
    public ICommand PreviewDragEnterCommand => _previewDragEnterCommand ?? (_previewDragEnterCommand = new RelayCommand(ExecutePreviewDragEnterCommand));

    private ICommand _keyDownCommand;

    /// <summary>
    /// KeyDown command
    /// </summary>
    public ICommand KeyDownCommand => _keyDownCommand ?? (_keyDownCommand = new RelayCommand(ExecuteKeyDownCommand));

    private ICommand _openInEditorCommand;

    /// <summary>
    /// Open in editor command
    /// </summary>
    public ICommand OpenInEditorCommand => _openInEditorCommand ?? (_openInEditorCommand = new RelayCommand(p => FileIsValid, p => ExecuteOpenInEditorCommand()));

    private ICommand _openTailDataFilterCommand;

    /// <summary>
    /// Open <see cref="TailData"/> filter
    /// </summary>
    public ICommand OpenTailDataFilterCommand => _openTailDataFilterCommand ?? (_openTailDataFilterCommand = new RelayCommand(p => FileIsValid, p => ExecuteOpenTailDataFilterCommand()));

    private IAsyncCommand _quickSaveCommand;

    /// <summary>
    /// Quick save command
    /// </summary>
    public IAsyncCommand QuickSaveCommand => _quickSaveCommand ?? (_quickSaveCommand = AsyncCommand.Create(p => CanExecuteQuickSaveCommand(), ExecuteQuickSaveCommandAsync));

    private ICommand _printTailDataCommand;

    /// <summary>
    /// Print <see cref="TailData"/>
    /// </summary>
    public ICommand PrintTailDataCommand => _printTailDataCommand ?? (_printTailDataCommand = new RelayCommand(p => CanExecutePrintTailDataCommand(), p => ExecutePrintTailDataCommand()));

    private ICommand _openSearchDialogCommand;

    /// <summary>
    /// Open search dialog command
    /// </summary>
    public ICommand OpenSearchDialogCommand => _openSearchDialogCommand ?? (_openSearchDialogCommand = new RelayCommand(p => CanExecuteOpenFontDialog(), p => ExecuteOpenSearchDialogCommand()));

    private ICommand _openFontDialogCommand;

    /// <summary>
    /// Open font dialog command
    /// </summary>
    public ICommand OpenFontDialogCommand => _openFontDialogCommand ?? (_openFontDialogCommand = new RelayCommand(p => CanExecuteOpenFontDialog(), p => ExecuteOpenFontDialogCommand()));

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Open font dialog command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteUnloadedCommand()));


    private IAsyncCommand _deleteHistoryCommand;

    /// <summary>
    /// Delete history command
    /// </summary>
    public IAsyncCommand DeleteHistoryCommand => _deleteHistoryCommand ?? (_deleteHistoryCommand = AsyncCommand.Create(p => CanExecuteDeleteHistoryCommand(), ExecuteDeleteHistoryCommandAsync));

    private ICommand _clearLogWindowCommand;

    /// <summary>
    /// Clear log window command
    /// </summary>
    public ICommand ClearLogWindowCommand => _clearLogWindowCommand ?? (_clearLogWindowCommand = new RelayCommand(p => CanExecuteClearLogWindowCommand(), p => ExecuteClearLogWindowCommand()));

    private ICommand _linesRefreshTimeChangedCommand;

    /// <summary>
    /// LinesRefreshTimeChanged command
    /// </summary>
    public ICommand LinesRefreshTimeChangedCommand => _linesRefreshTimeChangedCommand ?? (_linesRefreshTimeChangedCommand = new RelayCommand(ExecuteLinesRefreshTimeChangedCommand));

    #endregion

    #region Command functions

    private void ExecuteLinesRefreshTimeChangedCommand(object param)
    {
      if ( !(param is LinesRefreshTimeChangedArgs e) )
        return;

      OnLinesTimeChanged?.Invoke(this, e);
    }

    private bool CanExecuteDeleteHistoryCommand()
    {
      if ( !SettingsHelperController.CurrentSettings.SaveLogFileHistory )
        return false;

      return _historyQueueSet != null && _historyQueueSet.Count != 0;
    }

    private async Task ExecuteDeleteHistoryCommandAsync()
    {
      LogFileHistory.Clear();
      _historyQueueSet.Clear();

      MouseService.SetBusyState();
      await _historyController.DeleteHistoryAsync().ConfigureAwait(false);
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      SetCancellationTokenSource();

      if ( !SettingsHelperController.CurrentSettings.SaveLogFileHistory )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<DisableQuickAddInTailDataMessage>(OnDisableQuickAddFlag);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenTailDataMessage>(OnOpenTailData);

      _historyQueueSet = await _historyController.ReadXmlFileAsync().ConfigureAwait(false);
    }

    private void ExecuteUnloadedCommand()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<DisableQuickAddInTailDataMessage>(OnDisableQuickAddFlag);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenTailDataMessage>(OnOpenTailData);
    }

    private bool CanExecuteOpenFontDialog() => LogWindowState == EStatusbarState.FileLoaded || LogWindowState == EStatusbarState.Busy;

    private void ExecuteOpenFontDialogCommand()
    {
      if ( CurrentTailData == null )
        return;

      var fontManager = new FontChooseDialog
      {
        Owner = Window.GetWindow(this),
        SelectedFont = new FontInfo(CurrentTailData.FontType.FontFamily, CurrentTailData.FontType.FontSize, CurrentTailData.FontType.FontStyle,
          CurrentTailData.FontType.FontWeight, CurrentTailData.FontType.FontStretch)
      };

      if ( fontManager.ShowDialog() == true )
        CurrentTailData.FontType = fontManager.SelectedFont.FontType;
    }

    private bool CanExecuteClearLogWindowCommand() => SplitWindow.LogEntries != null && SplitWindow.LogEntries.Count != 0;

    private void ExecuteClearLogWindowCommand()
    {
      lock ( LogWindowControlLock )
      {
        MouseService.SetBusyState();
        SplitWindow.ClearItems();
        TailReader.ResetIndex();
      }
    }

    private void ExecuteOpenSearchDialogCommand()
    {

    }

    private bool CanExecutePrintTailDataCommand() => SplitWindow.LogEntries.Count != 0 && FileIsValid;

    private void ExecutePrintTailDataCommand() => _printerController.PrintDocument(SplitWindow.LogEntries, CurrentTailData);

    private bool CanExecuteQuickSaveCommand() => FileIsValid && CurrentTailData.OpenFromFileManager;

    private async Task ExecuteQuickSaveCommandAsync()
    {
      if ( !CurrentTailData.OpenFromFileManager )
        return;

      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
      await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, CurrentTailData).ConfigureAwait(false);
    }

    private void ExecuteOpenTailDataFilterCommand()
    {
      var filterManager = new FilterManager
      {
        Owner = Window.GetWindow(this)
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFilterDataFromTailDataMessage(this, CurrentTailData));
      filterManager.ShowDialog();

      OnPropertyChanged(nameof(CurrentTailData));
    }

    private void ExecuteOpenInEditorCommand()
    {
      if ( string.IsNullOrWhiteSpace(CurrentTailData.FileName) )
        return;

      var shellOpen = new ProcessStartInfo(CurrentTailData.FileName)
      {
        UseShellExecute = true
      };
      Process.Start(shellOpen);
    }

    private async Task ExecuteStartTailCommandAsync()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Visible;
      LogWindowState = EStatusbarState.Busy;

      TailReader.TailData = CurrentTailData;
      TailReader.StartTail();

      // If Logfile comes from the FileManager or settings does not allow to save the history, do not save it in the history
      if ( CurrentTailData.OpenFromFileManager || !SettingsHelperController.CurrentSettings.SaveLogFileHistory )
        return;

      if ( LogFileHistory.Contains(CurrentTailData.FileName) )
        return;

      MouseService.SetBusyState();
      SetCancellationTokenSource();
      await _historyController.SaveSearchHistoryAsync(CurrentTailData.FileName).ConfigureAwait(false);
    }

    private void ExecuteStopTailCommand()
    {
      if ( LogWindowTabItem == null )
        return;

      TailReader.StopTail();

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Collapsed;
      LogWindowState = string.IsNullOrWhiteSpace(CurrentTailData.FileName) ? EStatusbarState.Default : EStatusbarState.FileLoaded;
    }

    private void ExecuteOpenFileCommand()
    {
      LogFileComboBoxHasFocus = false;

      if ( !InteractionService.OpenFileDialog(out string fileName, "All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle) )
        return;

      SelectedItem = fileName;
      LogFileComboBoxHasFocus = true;
    }

    private void ExecuteAddToFileManagerCommand()
    {
      var quickAddManager = new QuickAdd
      {
        Owner = Window.GetWindow(this)
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new AddTailDataToQuickAddMessage(this, CurrentTailData));
      quickAddManager.ShowDialog();
    }

    private void ExecuteOpenFileManagerCommand() => OpenFileManager();

    private void ExecutePreviewDragEnterCommand(object parameter)
    {
      if ( !(parameter is DragEventArgs e) )
        return;

      e.Handled = true;
      e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
    }

    private void ExecuteKeyDownCommand(object parameter)
    {
      if ( !(parameter is KeyEventArgs e) )
        return;

      // We need this, that the workaround in SelectedItem works correct
      if ( e.Key != Key.Back )
        return;

      var myComboBox = e.Source as ComboBox;
      var textBox = (TextBox) myComboBox?.Template.FindName("PART_EditableTextBox", myComboBox);

      if ( !Equals(SelectedItem, textBox?.SelectedText) )
        return;

      CurrentTailData.FileName = string.Empty;
      SelectedItem = string.Empty;
      e.Handled = true;
    }

    #endregion

    #region HelperFunctions

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }

    private void SetCurrentLogFileName()
    {
      MouseService.SetBusyState();

      TailReader = new LogReadService();
      SplitWindow.LogReaderService = (LogReadService) TailReader;

      if ( !File.Exists(SelectedItem) )
      {
        LogWindowTabItem.HeaderContent = $"{Application.Current.TryFindResource("NoFile")}";
        LogWindowTabItem.HeaderToolTip = $"{Application.Current.TryFindResource("NoFile")}";
        LogWindowTabItem.TabItemBackgroundColorStringHex = DefaultEnvironmentSettings.TabItemHeaderBackgroundColor;

        CurrentTailData = new TailData();
        FileIsValid = false;
        LogWindowState = EStatusbarState.Default;
        return;
      }

      if ( !CurrentTailData.OpenFromFileManager || !Equals(SelectedItem, CurrentTailData.FileName) )
      {
        CurrentTailData = new TailData
        {
          FileName = SelectedItem,
          FileEncoding = EncodingDetector.GetEncodingAsync(SelectedItem).Result
        };
      }

      LogWindowTabItem.HeaderContent = CurrentTailData.File;
      LogWindowTabItem.HeaderToolTip = CurrentTailData.FileName;
      LogWindowTabItem.TabItemBackgroundColorStringHex = CurrentTailData.TabItemBackgroundColorStringHex;
      FileIsValid = true;

      if ( LogWindowTabItem.TabItemBusyIndicator != Visibility.Visible )
        LogWindowState = !string.IsNullOrWhiteSpace(CurrentTailData.FileName) ? EStatusbarState.FileLoaded : EStatusbarState.Default;
    }

    private void OpenFileManager()
    {
      var window = Window.GetWindow(this);

      if ( window != null )
      {
        var fileManager = new FileManager
        {
          Owner = window,
          ParentGuid = ((IDragWindow) window).DragWindowGuid
        };

        LogFileComboBoxHasFocus = false;

        fileManager.ShowDialog();
      }

      LogFileComboBoxHasFocus = WaitAsync().Result;
    }

    #endregion

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void TabItemTabHeaderBackgroundChanged(object sender, RoutedEventArgs e)
    {
      if ( !(sender is TabItem) )
        return;

      CurrentTailData.TabItemBackgroundColorStringHex = LogWindowTabItem.TabItemBackgroundColorStringHex;
    }

    private void SaveHistoryCompleted(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      LogFileHistory.Add(CurrentTailData.FileName);
      OnPropertyChanged(nameof(LogFileHistory));
    }

    private void LoadedCompleted(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( SettingsHelperController.CurrentSettings.SaveLogFileHistory )
      {
        foreach ( string s in _historyQueueSet )
        {
          if ( LogFileHistory.Contains(s) )
            continue;

          LogFileHistory.Add(s);
        }

        OnPropertyChanged(nameof(LogFileHistory));
      }

      SplitterPosition = 20;

      // Set focus to ComboBox
      LogFileComboBoxHasFocus = true;
    }

    private void OnOpenTailData(OpenTailDataMessage args)
    {
      if ( !(args.Sender is FileManagerViewModel) )
        return;

      var window = Window.GetWindow(this);

      if ( window == null )
        return;

      if ( args.TailData.NewWindow )
      {
        CreateDragWindow(args.TailData, window);
        return;
      }

      // Is it the right window?
      if ( ((IDragWindow) window).DragWindowGuid != args.ParentGuid )
        return;

      if ( LogWindowTabItem.TabItemBusyIndicator == Visibility.Visible )
      {
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataAsNewTabItem(this, args.TailData, args.ParentGuid));
        return;
      }

      CreateTailDataWindow(args.TailData);
    }

    private static void CreateDragWindow(TailData tailData, Window window)
    {
      DragWindow dragWindow;

      lock ( LogWindowControlLock )
      {
        const int offset = 100;
        tailData.OpenFromFileManager = true;
        ILogWindowControl content = new LogWindowControl
        {
          CurrentTailData = tailData
        };
        var tabItem = BusinessHelper.CreateDragSupportTabItem(tailData.File, tailData.FileName, Visibility.Collapsed, content);
        dragWindow = DragWindow.CreateTabWindow(window.Left + offset, window.Top + offset, window.Width, window.Height, tabItem);

        // Unregister tab item, we do not need it again!
        BusinessHelper.UnregisterTabItem(tabItem);
      }

      dragWindow?.Activate();
      dragWindow?.Focus();
    }

    /// <summary>
    /// Create tail data window
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    public void CreateTailDataWindow(TailData item)
    {
      CurrentTailData = item;
      CurrentTailData.OpenFromFileManager = true;
      SelectedItem = CurrentTailData.FileName;

      OnPropertyChanged(nameof(CurrentTailData));
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public async Task DisposeAsync()
    {
      LOG.Trace("Dispose useless resources");

      MouseService.SetBusyState();

      if ( TailReader.IsBusy )
      {
        TailReader.StopTail();

        while ( TailReader.IsBusy )
        {
          await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
        }
      }

      lock ( LogWindowControlLock )
      {
        SplitWindow.LogEntries = null;
        CurrentTailData = null;
      }
    }

    private async Task<bool> WaitAsync()
    {
      // Wait some ms to set the correct focus
      await Task.Delay(TimeSpan.FromMilliseconds(25)).ConfigureAwait(false);
      return true;
    }

    private void OnDisableQuickAddFlag(DisableQuickAddInTailDataMessage args)
    {
      if ( !(args.Sender is QuickAddViewModel) )
        return;

      CurrentTailData.OpenFromFileManager = args.OpenFromFileManager;
    }

    /// <summary>
    /// On file drop
    /// </summary>
    /// <param name="filePaths">Array of file pathes</param>
    public void OnFileDrop(string[] filePaths)
    {
      if ( filePaths.Length == 0 )
        return;

      try
      {
        string fileName = filePaths.First();
        string extension = Path.GetExtension(fileName);

        if ( string.IsNullOrWhiteSpace(extension) )
          return;

        SelectedItem = fileName;
        OnPropertyChanged(nameof(CurrentTailData));

        LogFileComboBoxHasFocus = true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }
  }
}
