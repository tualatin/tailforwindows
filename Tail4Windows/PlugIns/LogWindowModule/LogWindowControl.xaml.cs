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
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.WindowsEventReadModule.Events.Args;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.Keybindings;
using Org.Vs.TailForWin.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.PlugIns.FontChooserModule;
using Org.Vs.TailForWin.PlugIns.GoToLineModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.PatternModule;
using Org.Vs.TailForWin.PlugIns.QuickAddModule;
using Org.Vs.TailForWin.PlugIns.QuickAddModule.ViewModels;
using Org.Vs.TailForWin.PlugIns.WindowEventReadModule;
using Org.Vs.TailForWin.PlugIns.WindowEventReadModule.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;
using Org.Vs.TailForWin.UI.Utils;


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
      WindowId = Guid.NewGuid();

      ((AsyncCommand<object>) StartTailCommand).PropertyChanged += SaveHistoryCompleted;
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedCompleted;
      SettingsHelperController.CurrentSettings.PropertyChanged += CurrentSettingsPropertyChanged;
    }

    /// <summary>
    /// <see cref="ILogReadService"/>
    /// </summary>
    public ILogReadService TailReader
    {
      get;
      private set;
    }

    /// <summary>
    /// IsSmartWatch and Autorun activated
    /// </summary>
    public bool IsSmartWatchAutoRun
    {
      get;
      set;
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

    /// <summary>
    /// Gets parent window id
    /// </summary>
    public Guid ParentWindowId
    {
      get
      {
        var window = Window.GetWindow(this);
        return ((IDragWindow) window)?.DragWindowGuid ?? Guid.Empty;
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

    /// <summary>
    /// Window id
    /// </summary>
    public Guid WindowId
    {
      get;
    }

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
    public ICommand AddToFileManagerCommand => _addToFileManagerCommand ?? (_addToFileManagerCommand = new RelayCommand(p => CanExecuteAddToFileManager(), p => ExecuteAddToFileManagerCommand()));

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

    private ICommand _patternControlCommand;

    /// <summary>
    /// Open pattern util command
    /// </summary>
    public ICommand PatternControlCommand => _patternControlCommand ?? (_patternControlCommand = new RelayCommand(p => FileIsValid, p => ExecutePatternControlCommand()));

    private ICommand _smartWatchCommand;

    /// <summary>
    /// SmartWatch command
    /// </summary>
    public ICommand SmartWatchCommand => _smartWatchCommand ?? (_smartWatchCommand = new RelayCommand(p => CanExecuteSmartWatchCommand(), p => ExecuteSmartWatchCommand()));

    private ICommand _openWindowsEventsCommand;

    /// <summary>
    /// Open Windows events command
    /// </summary>
    public ICommand OpenWindowsEventsCommand => _openWindowsEventsCommand ?? (_openWindowsEventsCommand = new RelayCommand(p => LogWindowState != EStatusbarState.Busy, p => ExecuteOpenWindowsEventsCommand()));

    #endregion

    #region Command functions

    private void ExecuteOpenWindowsEventsCommand()
    {
      var windowsEventCategories = new WindowsEventCategories
      {
        Owner = Window.GetWindow(this)
      };

      if ( windowsEventCategories.WindowsEventCategoriesViewModel != null )
        windowsEventCategories.WindowsEventCategoriesViewModel.OnOpenWindowsEvent += WindowsEventCategoriesOnOpenWindowsEvent;

      windowsEventCategories.ShowDialog();
    }

    private void WindowsEventCategoriesOnOpenWindowsEvent(object sender, OnOpenWindowsEventArgs e)
    {
      if ( !(sender is IWindowsEventCategoriesViewModel) )
        return;

      SetWindowsEventTailReader(e.TailData);
    }

    private bool CanExecuteSmartWatchCommand() => SettingsHelperController.CurrentSettings.SmartWatch && FileIsValid;

    private void ExecuteSmartWatchCommand()
    {
      if ( !TailReader.IsBusy )
        return;

      if ( CurrentTailData.SmartWatch && !TailReader.SmartWatch.IsBusy )
      {
        BusinessHelper.CreatePopUpWindow("SmartWatch", Application.Current.TryFindResource("SmartWatchStart").ToString());
        TailReader.SmartWatch.StartSmartWatch(CurrentTailData);
      }
      else if ( !CurrentTailData.SmartWatch && TailReader.SmartWatch.IsBusy )
      {
        BusinessHelper.CreatePopUpWindow("SmartWatch", Application.Current.TryFindResource("SmartWatchSuspend").ToString());
        TailReader.SmartWatch.SuspendSmartWatch();
      }
      else if ( CurrentTailData.SmartWatch && TailReader.SmartWatch.IsBusy )
      {
        BusinessHelper.CreatePopUpWindow("SmartWatch", Application.Current.TryFindResource("SmartWatchResume").ToString());
        TailReader.SmartWatch.StartSmartWatch(CurrentTailData);
      }
    }

    private void ExecutePatternControlCommand()
    {
      var patternControl = new PatternControl
      {
        Owner = Window.GetWindow(this),
        CurrentTailData = CurrentTailData
      };
      patternControl.ShowDialog();
    }

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
      RegisterKeybindingEvents();

      if ( !SettingsHelperController.CurrentSettings.SaveLogFileHistory )
        return;

      _historyQueueSet = await _historyController.ReadXmlFileAsync().ConfigureAwait(false);
    }

    private void RegisterKeybindingEvents()
    {
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<DisableQuickAddInTailDataMessage>(OnDisableQuickAddFlag);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenTailDataMessage>(OnOpenTailData);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenGoToLineDialogMessage>(OnOpenGoToLineDialog);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenTailManagerMessage>(OnOpenTailManager);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFilterManagerMessage>(OnOpenFilterManager);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ToggleFilterMessage>(OnToggleFilter);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<QuickSaveMessage>(OnQuickSave);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFileMessage>(OnOpenFile);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ClearTailLogMessage>(OnClearTailLog);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<StartTailMessage>(OnStartTail);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<StopTailMessage>(OnStopTail);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<QuickAddMessage>(OnQuickAdd);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFontSettingsMessage>(OnOpenFontSettings);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenWindowsEventMessage>(OnOpenWindowsEvent);
    }

    private void ExecuteUnloadedCommand() => UnregisterKeybindingEvents();

    private void UnregisterKeybindingEvents()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<DisableQuickAddInTailDataMessage>(OnDisableQuickAddFlag);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenTailDataMessage>(OnOpenTailData);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenGoToLineDialogMessage>(OnOpenGoToLineDialog);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenTailManagerMessage>(OnOpenTailManager);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenFilterManagerMessage>(OnOpenFilterManager);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<ToggleFilterMessage>(OnToggleFilter);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<QuickSaveMessage>(OnQuickSave);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenFileMessage>(OnOpenFile);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<ClearTailLogMessage>(OnClearTailLog);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartTailMessage>(OnStartTail);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StopTailMessage>(OnStopTail);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<QuickAddMessage>(OnQuickAdd);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenFontSettingsMessage>(OnOpenFontSettings);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenWindowsEventMessage>(OnOpenWindowsEvent);
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

    private void ExecuteOpenSearchDialogCommand() => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFindWhatWindowMessage(this, CurrentTailData.File, WindowId));

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
      if ( !CurrentTailData.IsWindowsEvent && string.IsNullOrWhiteSpace(CurrentTailData.FileName) )
        return;

      if ( !CurrentTailData.IsWindowsEvent )
      {
        var shellOpen = new ProcessStartInfo(CurrentTailData.FileName)
        {
          UseShellExecute = true
        };
        Process.Start(shellOpen);
      }
      else
      {
        Process.Start("eventvwr");
      }
    }

    private async Task ExecuteStartTailCommandAsync()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Visible;
      LogWindowState = EStatusbarState.Busy;
      CurrentTailData.OpenFromSmartWatch = false;
      TailReader.TailData = CurrentTailData;

      TailReader.StartTail();

      // If Logfile comes from the FileManager or settings does not allow to save the history or is WindowsEvent setting, do not save it in the history
      if ( CurrentTailData.OpenFromFileManager || !SettingsHelperController.CurrentSettings.SaveLogFileHistory || CurrentTailData.IsWindowsEvent )
        return;

      if ( LogFileHistory.Contains(CurrentTailData.FileName) )
        return;

      MouseService.SetBusyState();
      SetCancellationTokenSource();
      await _historyController.SaveSearchHistoryAsync(CurrentTailData.FileName).ConfigureAwait(false);
    }

    /// <summary>
    /// Stops current TailReader
    /// </summary>
    public void ExecuteStopTailCommand()
    {
      if ( LogWindowTabItem == null )
        return;

      TailReader.StopTail();
      NotifyTaskCompletion task = NotifyTaskCompletion.Create(WaitingForTailWorkerAsync);
      task.PropertyChanged += OnWaitingForTailWorkerPropertyChanged;

      if ( CurrentTailData.IsWindowsEvent )
        LogWindowState = string.IsNullOrWhiteSpace(CurrentTailData.WindowsEvent.Category) ? EStatusbarState.Default : EStatusbarState.FileLoaded;
      else
        LogWindowState = string.IsNullOrWhiteSpace(CurrentTailData.FileName) ? EStatusbarState.Default : EStatusbarState.FileLoaded;
    }

    private void OnWaitingForTailWorkerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Collapsed;
    }

    private void ExecuteOpenFileCommand()
    {
      LogFileComboBoxHasFocus = false;

      if ( !InteractionService.OpenFileDialog(out string fileName, "All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle) )
        return;

      SelectedItem = fileName;
      LogFileComboBoxHasFocus = true;
    }

    private bool CanExecuteAddToFileManager() => FileIsValid && CurrentTailData != null && !CurrentTailData.OpenFromFileManager;

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

      if ( TailReader != null && !CurrentTailData.OpenFromSmartWatch )
        TailReader = null;

      if ( !CurrentTailData.OpenFromSmartWatch )
      {
        TailReader = new LogReadService();
        SplitWindow.LogReaderService = TailReader;
      }

      if ( !File.Exists(SelectedItem) )
      {
        LogWindowTabItem.HeaderContent = $"{Application.Current.TryFindResource("NoFile")}";
        LogWindowTabItem.HeaderToolTip = $"{Application.Current.TryFindResource("NoFile")}";
        LogWindowTabItem.TabItemBackgroundColorStringHex = DefaultEnvironmentSettings.TabItemHeaderBackgroundColor;

        CurrentTailData = new TailData();
        FileIsValid = false;
        LogWindowState = EStatusbarState.Default;
        SplitWindow.CurrentTailData = CurrentTailData;
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
      SplitWindow.CurrentTailData = CurrentTailData;

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
          ParentGuid = ParentWindowId
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


      if ( !IsSmartWatchAutoRun )
        return;

      NotifyTaskCompletion.Create(ExecuteStartTailCommandAsync);
      IsSmartWatchAutoRun = false;
    }

    private void OnOpenTailData(OpenTailDataMessage args)
    {
      if ( args.Sender == null )
        return;

      var window = Window.GetWindow(this);

      if ( window == null )
        return;

      if ( args.IsSmartWatch )
      {
        // Is it the right window?
        if ( ParentWindowId != args.ParentGuid )
          return;

        // Open new SmartWatch object
        OpenSmartWatchTailData(args.TailData);
        return;
      }

      // Open in new Drag window
      if ( args.TailData.NewWindow )
      {
        CreateDragWindow(args.TailData, window);
        return;
      }

      // Is it the right window?
      if ( ParentWindowId != args.ParentGuid )
        return;

      if ( LogWindowTabItem.TabItemBusyIndicator == Visibility.Visible )
      {
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataAsNewTabItem(this, args.TailData, args.ParentGuid, false));
        return;
      }

      CreateTailDataWindow(args.TailData);
    }

    private void OpenSmartWatchTailData(TailData tailData)
    {
      MouseService.SetBusyState();

      if ( !TailReader.IsBusy )
        return;

      TailReader.StopTail();

      NotifyTaskCompletion task = NotifyTaskCompletion.Create(WaitingForTailWorkerAsync);
      task.PropertyChanged += OnWaitingForTailWorkerSmartWatchPropertyChanged;
      tailData.OpenFromSmartWatch = true;

      CreateTailDataWindow(tailData);
    }

    private void OnWaitingForTailWorkerSmartWatchPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( CurrentTailData.AutoRun )
        NotifyTaskCompletion.Create(ExecuteStartTailCommandAsync());
    }

    private async Task WaitingForTailWorkerAsync()
    {
      while ( TailReader.IsBusy )
      {
        await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
      }
    }

    private async Task WaitingForWorkersAsync()
    {
      while ( TailReader.IsBusy || TailReader.SmartWatch.IsBusy )
      {
        await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
      }
    }

    private static void CreateDragWindow(TailData tailData, Window window)
    {
      DragWindow dragWindow;

      lock ( LogWindowControlLock )
      {
        const int offset = 100;
        ILogWindowControl content = new LogWindowControl
        {
          CurrentTailData = tailData
        };
        DragSupportTabItem tabItem = BusinessHelper.CreateDragSupportTabItem(tailData.File, tailData.FileName, Visibility.Collapsed, content);
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
      if ( item.IsWindowsEvent )
      {
        SetWindowsEventTailReader(item);
        return;
      }

      CurrentTailData = item;
      CurrentTailData.OpenFromFileManager = true;
      SelectedItem = CurrentTailData.FileName;

      OnPropertyChanged(nameof(CurrentTailData));
    }

    /// <summary>
    /// Set Windows event tail reader
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    public void SetWindowsEventTailReader(TailData item)
    {
      if ( TailReader != null )
        TailReader = null;

      TailReader = new WindowsEventReadService
      {
        TailData = CurrentTailData
      };
      CurrentTailData = item;
      SplitWindow.LogReaderService = TailReader;
      string machine = CurrentTailData.WindowsEvent.Machine == "." ? Environment.MachineName : CurrentTailData.WindowsEvent.Machine;
      LogWindowTabItem.HeaderContent = $"{machine}: {CurrentTailData.WindowsEvent.Name}";
      LogWindowTabItem.HeaderToolTip = $"{machine}: {CurrentTailData.WindowsEvent.Name}";
      LogWindowTabItem.TabItemBackgroundColorStringHex = CurrentTailData.TabItemBackgroundColorStringHex;
      FileIsValid = true;
      SplitWindow.CurrentTailData = CurrentTailData;

      if ( LogWindowTabItem.TabItemBusyIndicator != Visibility.Visible )
        LogWindowState = !string.IsNullOrWhiteSpace(CurrentTailData.WindowsEvent.Category) ? EStatusbarState.FileLoaded : EStatusbarState.Default;
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

        if ( !CurrentTailData.IsWindowsEvent )
        {
          TailReader.SmartWatch.StopSmartWatch();
          await WaitingForWorkersAsync().ConfigureAwait(false);
        }
        else
        {
          await WaitingForTailWorkerAsync().ConfigureAwait(false);
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

    private void CurrentSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !Equals(e.PropertyName, "SmartWatch") )
        return;

      if ( TailReader.SmartWatch == null )
        return;

      if ( !SettingsHelperController.CurrentSettings.SmartWatch && TailReader.IsBusy && !TailReader.SmartWatch.IsSuspended )
      {
        BusinessHelper.CreatePopUpWindow("SmartWatch", Application.Current.TryFindResource("SmartWatchSuspend").ToString());
        TailReader.SmartWatch.SuspendSmartWatch();
      }
      else if ( SettingsHelperController.CurrentSettings.SmartWatch && TailReader.IsBusy && TailReader.SmartWatch.IsSuspended && CurrentTailData.SmartWatch )
      {
        BusinessHelper.CreatePopUpWindow("SmartWatch", Application.Current.TryFindResource("SmartWatchResume").ToString());
        TailReader.SmartWatch.StartSmartWatch(CurrentTailData);
      }
    }

    #region Keybindings

    private void OnOpenWindowsEvent(OpenWindowsEventMessage args)
    {
      if ( args.WindowGuid != WindowId )
        return;

      ExecuteOpenWindowsEventsCommand();
    }

    private void OnOpenFontSettings(OpenFontSettingsMessage args)
    {
      if ( args.WindowGuid != WindowId || !CanExecuteOpenFontDialog() )
        return;

      ExecuteOpenFontDialogCommand();
    }

    private void OnQuickAdd(QuickAddMessage args)
    {
      if ( FileIsValid && CurrentTailData != null && !CurrentTailData.OpenFromFileManager && args.WindowGuid == WindowId )
        ExecuteAddToFileManagerCommand();
    }

    private void OnStopTail(StopTailMessage args)
    {
      if ( args.WindowGuid != WindowId || LogWindowState != EStatusbarState.Busy )
        return;

      ExecuteStopTailCommand();
    }

    private void OnStartTail(StartTailMessage args)
    {
      if ( args.WindowGuid != WindowId || !FileIsValid || LogWindowState == EStatusbarState.Busy )
        return;

      NotifyTaskCompletion.Create(ExecuteStartTailCommandAsync);
    }

    private void OnClearTailLog(ClearTailLogMessage args)
    {
      if ( args.WindowGuid != WindowId )
        return;

      ExecuteClearLogWindowCommand();
    }

    private void OnOpenFile(OpenFileMessage args)
    {
      if ( args.WindowGuid != WindowId )
        return;

      ExecuteOpenFileCommand();
    }

    private void OnQuickSave(QuickSaveMessage args)
    {
      if ( args.WindowGuid != WindowId || !CanExecuteQuickSaveCommand() )
        return;

      NotifyTaskCompletion.Create(ExecuteQuickSaveCommandAsync());
    }

    private void OnToggleFilter(ToggleFilterMessage args)
    {
      if ( args.WindowGuid != WindowId && CurrentTailData != null && CurrentTailData.ListOfFilter.Count > 0 )
        return;

      if ( CurrentTailData != null )
        CurrentTailData.FilterState = !CurrentTailData.FilterState;
    }

    private void OnOpenFilterManager(OpenFilterManagerMessage args)
    {
      if ( args.WindowGuid != WindowId || !FileIsValid )
        return;

      ExecuteOpenTailDataFilterCommand();
    }

    private void OnOpenTailManager(OpenTailManagerMessage args)
    {
      if ( args.WindowGuid != WindowId )
        return;

      OpenFileManager();
    }

    private void OnOpenGoToLineDialog(OpenGoToLineDialogMessage args)
    {
      if ( !CanExecuteOpenFontDialog() || LinesRead == 0 )
        return;

      if ( ParentWindowId != args.ParentGuid )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<GoToLineMessage>(OnGoToLine);
      var goToLine = new GoToLine(SplitWindow.LogEntries[0].Index, LinesRead, args.ParentGuid)
      {
        Owner = Window.GetWindow(this),
        ShouldClose = true
      };
      goToLine.ShowDialog();

      new ThrottledExecution().InMs(100).Do(() =>
      {
        // Unregister message, we do not need it again!
        EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<GoToLineMessage>(OnGoToLine);
      });
    }

    private void OnGoToLine(GoToLineMessage args)
    {
      if ( ParentWindowId != args.ParentGuid )
        return;

      SplitWindow.GoToLine(args.Index);
    }

    #endregion

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
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
  }
}
