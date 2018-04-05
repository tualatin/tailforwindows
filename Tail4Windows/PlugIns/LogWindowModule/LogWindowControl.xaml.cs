﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
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
  public partial class LogWindowControl : ILogWindow, IFileDragDropTarget
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogWindowControl));

    private readonly IXmlFileManager _xmlFileManagerReader;

    private string _currentLogFile;

    /// <summary>
    /// Current log file
    /// </summary>
    public string CurrentLogFile
    {
      get => _currentLogFile;
      set
      {
        if ( Equals(value, _currentLogFile) )
          return;

        _currentLogFile = value;
        CurrenTailData.FileName = value;
        LogWindowState = !string.IsNullOrWhiteSpace(_currentLogFile) ? EStatusbarState.FileLoaded : EStatusbarState.Default;

        OnPropertyChanged();
      }
    }

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

      _xmlFileManagerReader = new XmlFileManagerController();
      CurrentLogFile = string.Empty;
    }

    /// <summary>
    /// LogWindowTabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem LogWindowTabItem
    {
      get;
      set;
    }

    private EStatusbarState _logWindowState;

    /// <summary>
    /// Current LogWindowState <see cref="EStatusbarState"/>
    /// </summary>
    public EStatusbarState LogWindowState
    {
      get => _logWindowState;
      private set
      {
        if ( value == _logWindowState )
          return;

        _logWindowState = value;

        if (IsSelected)
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

    #region Commands

    private ICommand _openFileCommand;

    /// <summary>
    /// Open file command
    /// </summary>
    public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(p => ExecuteOpenFileCommand()));

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

    private IAsyncCommand _addToFileManagerCommand;

    /// <summary>
    /// Add to FileManager command
    /// </summary>
    public IAsyncCommand AddToFileManagerCommand => _addToFileManagerCommand ?? (_addToFileManagerCommand = AsyncCommand.Create(ExecuteAddToFileManagerCommandAsync));

    private IAsyncCommand _openFileManagerCommand;

    /// <summary>
    /// Open FileManager command
    /// </summary>
    public IAsyncCommand OpenFileManagerCommand => _openFileManagerCommand ?? (_openFileManagerCommand = AsyncCommand.Create(ExecuteOpenFileManagerCommandAsync));

    private ICommand _previewDragEnterCommand;

    /// <summary>
    /// Preview drag enter command
    /// </summary>
    public ICommand PreviewDragEnterCommand => _previewDragEnterCommand ?? (_previewDragEnterCommand = new RelayCommand(ExecutePreviewDragEnterCommand));

    #endregion

    #region Command functions

    private void ExecuteStartTailCommand()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Visible;
      LogWindowState = EStatusbarState.Busy;
    }

    private void ExecuteStopTailCommand()
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Collapsed;
      LogWindowState = string.IsNullOrWhiteSpace(CurrentLogFile) ? EStatusbarState.Default : EStatusbarState.FileLoaded;
    }

    private void ExecuteOpenFileCommand()
    {
      if ( FileOpenDialog.OpenDialog("All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle, out string fileName) )
        CurrentLogFile = fileName;
    }

    private async Task ExecuteAddToFileManagerCommandAsync()
    {
      await OpenFileManagerAsync();
    }

    private async Task ExecuteOpenFileManagerCommandAsync() => await OpenFileManagerAsync();

    private void ExecutePreviewDragEnterCommand(object parameter)
    {
      if ( !(parameter is DragEventArgs e) )
        return;

      e.Handled = true;
      e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
    }

    #endregion

    #region HelperFunctions

    private async Task OpenFileManagerAsync(TailData tailData = null)
    {
      MouseService.SetBusyState();

      ObservableCollection<TailData> result = null;

      try
      {
        result = await _xmlFileManagerReader.ReadXmlFileAsync().ConfigureAwait(false);
      }
      catch
      {
        // Nothing
      }

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
        string extension = System.IO.Path.GetExtension(fileName);

        if ( string.IsNullOrWhiteSpace(extension) )
          return;

        CurrentLogFile = fileName;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }
  }
}
