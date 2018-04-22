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
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Controller;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.QuickAddModule;
using Org.Vs.TailForWin.PlugIns.QuickAddModule.ViewModels;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;
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

    private CancellationTokenSource _cts;
    private readonly IXmlSearchHistory<QueueSet<string>> _historyController;
    private readonly IXmlFileManager _xmlFileManagerController;
    private QueueSet<string> _historyQueueSet;

    #region Events

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

      _historyController = new XmlHistoryController();
      _xmlFileManagerController = new XmlFileManagerController();

      ((AsyncCommand<object>) StartTailCommand).PropertyChanged += SaveHistoryCompleted;
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedCompleted;
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
          OnStatusChanged?.Invoke(this, new StatusChangedArgs(LogWindowState, CurrentTailData.FileEncoding));

        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Is selected
    /// </summary>
    public bool IsSelected => LogWindowTabItem != null && LogWindowTabItem.IsSelected;

    /// <summary>
    /// Current tail data <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get;
      set;
    } = new TailData();

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
    public ICommand AddToFileManagerCommand => _addToFileManagerCommand ?? (_addToFileManagerCommand = new RelayCommand(p => FileIsValid && !CurrentTailData.OpenFromFileManager, p => ExecuteAddToFileManagerCommand()));

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

    private IAsyncCommand _printTailDataCommand;

    /// <summary>
    /// Print <see cref="TailData"/>
    /// </summary>
    public IAsyncCommand PrintTailDataCommand => _printTailDataCommand ?? (_printTailDataCommand = AsyncCommand.Create(p => FileIsValid, ExecutePrintTailDataCommandAsync));

    private ICommand _openSearchDialogCommand;

    /// <summary>
    /// Open search dialog command
    /// </summary>
    public ICommand OpenSearchDialogCommand => _openSearchDialogCommand ?? (_openSearchDialogCommand = new RelayCommand(p => ExecuteOpenSearchDialogCommand()));

    private ICommand _openFontDialogCommand;

    /// <summary>
    /// Open font dialog command
    /// </summary>
    public ICommand OpenFontDialogCommand => _openFontDialogCommand ?? (_openFontDialogCommand = new RelayCommand(p => ExecuteOpenFontDialogCommand()));

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

    #endregion

    #region Command functions

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

    private void ExecuteOpenFontDialogCommand()
    {

    }

    private void ExecuteOpenSearchDialogCommand()
    {

    }

    private async Task ExecutePrintTailDataCommandAsync()
    {

    }

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
      FilterManager filterManager = new FilterManager
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

      // If Logfile comes from the FileManager or settings does not allow to save the history, do not save it in the history
      if ( CurrentTailData.OpenFromFileManager || !SettingsHelperController.CurrentSettings.SaveLogFileHistory )
        return;

      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await _historyController.SaveSearchHistoryAsync(CurrentTailData.FileName).ConfigureAwait(false);
    }

    private void ExecuteStopTailCommand()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Collapsed;
      LogWindowState = string.IsNullOrWhiteSpace(CurrentTailData.File) ? EStatusbarState.Default : EStatusbarState.FileLoaded;
    }

    private void ExecuteOpenFileCommand()
    {
      LogFileComboBoxHasFocus = false;

      if ( !FileOpenDialog.OpenDialog("All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle, out string fileName) )
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

      // Is it the right window?
      if ( ((IDragWindow) window).DragWindowGuid != args.ParentGuid )
        return;

      if ( LogWindowTabItem.TabItemBusyIndicator == Visibility.Visible )
      {
        return;
      }

      CreateTailDataWindow(args.TailData);
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
