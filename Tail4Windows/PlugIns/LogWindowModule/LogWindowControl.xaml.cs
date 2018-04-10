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
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Controller;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for LogWindowControl.xaml
  /// </summary>
  public partial class LogWindowControl : ILogWindowControl, IFileDragDropTarget
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogWindowControl));

    private readonly CancellationTokenSource _cts;
    private readonly IXmlSearchHistory<QueueSet<string>> _historyController;
    private readonly IXmlFileManager _xmlFileManagerController;

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

      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
      _historyController = new XmlHistoryController();
      _xmlFileManagerController = new XmlFileManagerController();

      ((AsyncCommand<object>) StartTailCommand).PropertyChanged += SaveHistoryCompleted;
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
        if ( value == _logWindowState )
          return;

        _logWindowState = value;

        if ( IsSelected )
          OnStatusChanged?.Invoke(this, new StatusChangedArgs(LogWindowState));

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
    public TailData CurrenTailData
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
        _selectedItem = value;
        OnPropertyChanged();
      }
    }

    #region Commands

    private ICommand _openFileCommand;

    /// <summary>
    /// Open file command
    /// </summary>
    public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(p => ExecuteOpenFileCommand()));

    private IAsyncCommand _startTailCommand;

    /// <summary>
    /// Start tail command
    /// </summary>
    public IAsyncCommand StartTailCommand => _startTailCommand ?? (_startTailCommand = AsyncCommand.Create(ExecuteStartTailCommandAsync));

    private ICommand _stopTailCommand;

    /// <summary>
    /// Stop tail command
    /// </summary>
    public ICommand StopTailCommand => _stopTailCommand ?? (_stopTailCommand = new RelayCommand(p => ExecuteStopTailCommand()));

    private ICommand _addToFileManagerCommand;

    /// <summary>
    /// Add to FileManager command
    /// </summary>
    public ICommand AddToFileManagerCommand => _addToFileManagerCommand ?? (_addToFileManagerCommand = new RelayCommand(p => ExecuteAddToFileManagerCommand()));

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

    private ICommand _logFileTextBoxTextChangedCommand;

    /// <summary>
    /// LogFile text box text changed command
    /// </summary>
    public ICommand LogFileTextBoxTextChangedCommand => _logFileTextBoxTextChangedCommand ?? (_logFileTextBoxTextChangedCommand = new RelayCommand(ExecuteLogFileTextBoxTextChangedCommand));

    private ICommand _openInEditorCommand;

    /// <summary>
    /// Open in editor command
    /// </summary>
    public ICommand OpenInEditorCommand => _openInEditorCommand ?? (_openInEditorCommand = new RelayCommand(p => ExecuteOpenInEditorCommand()));

    private ICommand _openTailDataFilterCommand;

    /// <summary>
    /// Open <see cref="TailData"/> filter
    /// </summary>
    public ICommand OpenTailDataFilterCommand => _openTailDataFilterCommand ?? (_openTailDataFilterCommand = new RelayCommand(p => ExecuteOpenTailDataFilterCommand()));

    private IAsyncCommand _quickSaveCommand;

    /// <summary>
    /// Quick save command
    /// </summary>
    public IAsyncCommand QuickSaveCommand => _quickSaveCommand ?? (_quickSaveCommand = AsyncCommand.Create(ExecuteQuickSaveCommandAsync));

    private IAsyncCommand _printTailDataCommand;

    /// <summary>
    /// Print <see cref="TailData"/>
    /// </summary>
    public IAsyncCommand PrintTailDataCommand => _printTailDataCommand ?? (_printTailDataCommand = AsyncCommand.Create(ExecutePrintTailDataCommandAsync));

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

    #endregion

    #region Command functions

    private async Task<ObservableCollection<string>> ExecuteLoadedCommandAsync()
    {
      var result = await _historyController.ReadXmlFileAsync().ConfigureAwait(false);
      
      foreach ( string s in result )
      {
        LogFileHistory.Add(s);
      }
      return LogFileHistory;
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

    private async Task ExecuteQuickSaveCommandAsync()
    {
      if ( !CurrenTailData.OpenFromFileManager )
        return;

      await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, CurrenTailData).ConfigureAwait(false);
    }

    private void ExecuteOpenTailDataFilterCommand()
    {

    }

    private void ExecuteOpenInEditorCommand()
    {
      if ( string.IsNullOrWhiteSpace(CurrenTailData.FileName) )
        return;

      var shellOpen = new ProcessStartInfo(CurrenTailData.FileName)
      {
        UseShellExecute = true
      };
      Process.Start(shellOpen);
    }

    private void ExecuteLogFileTextBoxTextChangedCommand(object param)
    {
      if ( !(param is TextChangedEventArgs e) )
        return;

      SetCurrentLogFileName();
    }

    private async Task ExecuteStartTailCommandAsync()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Visible;
      LogWindowState = EStatusbarState.Busy;

      // If Logfile comes from the FileManager, do not save it in the history
      if ( CurrenTailData.OpenFromFileManager )
        return;

      MouseService.SetBusyState();
      await _historyController.SaveSearchHistoryAsync(CurrenTailData.FileName).ConfigureAwait(false);
    }

    private void ExecuteStopTailCommand()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Collapsed;
      LogWindowState = string.IsNullOrWhiteSpace(CurrenTailData.File) ? EStatusbarState.Default : EStatusbarState.FileLoaded;
    }

    private void ExecuteOpenFileCommand()
    {
      LogFileComboBoxHasFocus = false;

      if ( !FileOpenDialog.OpenDialog("All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle, out string fileName) )
        return;

      CurrenTailData.FileName = fileName;
      LogFileComboBoxHasFocus = true;

      OnPropertyChanged(nameof(CurrenTailData));
    }

    private void ExecuteAddToFileManagerCommand() => OpenFileManager(CurrenTailData);

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

    private void SetCurrentLogFileName()
    {
      if ( !File.Exists(CurrenTailData.FileName) )
      {
        LogWindowTabItem.HeaderContent = $"{Application.Current.TryFindResource("NoFile")}";
        LogWindowTabItem.HeaderToolTip = $"{Application.Current.TryFindResource("NoFile")}";

        CurrenTailData = new TailData();
        OnPropertyChanged(nameof(CurrenTailData));

        FileIsValid = false;
        LogWindowState = EStatusbarState.Default;
        return;
      }

      LogWindowTabItem.HeaderContent = CurrenTailData.File;
      LogWindowTabItem.HeaderToolTip = CurrenTailData.FileName;
      FileIsValid = true;
      LogWindowState = !string.IsNullOrWhiteSpace(CurrenTailData.FileName) ? EStatusbarState.FileLoaded : EStatusbarState.Default;
    }

    private void OpenFileManager(TailData tailData = null)
    {
      var fileManager = new FileManager
      {
        Owner = Application.Current.MainWindow
      };
      fileManager.ShowDialog();
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

      CurrenTailData.TabItemBackgroundColorStringHex = LogWindowTabItem.TabItemBackgroundColorStringHex;
    }

    private void SaveHistoryCompleted(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      LogFileHistory.Add(CurrenTailData.FileName);
      OnPropertyChanged(nameof(LogFileHistory));
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

        CurrenTailData.FileName = fileName;
        LogFileComboBoxHasFocus = true;

        OnPropertyChanged(nameof(CurrenTailData));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }
  }
}
