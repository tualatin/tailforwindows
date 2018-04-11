using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels
{
  /// <summary>
  /// FileManager view model
  /// </summary>
  public class FileManagerViewModel : NotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileManagerViewModel));

    private TailData.MementoTailData _mementoTailData;
    private readonly CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;

    #region Properties

    /// <summary>
    /// FileManager collection
    /// </summary>
    public ObservableCollection<TailData> FileManagerCollection
    {
      get;
      set;
    } = new ObservableCollection<TailData>();

    private TailData _selectedItem;

    /// <summary>
    /// Current selected item
    /// </summary>
    public TailData SelectedItem
    {
      get => _selectedItem;
      set
      {
        _selectedItem = value;
        OnPropertyChanged();
      }
    }

    private bool _dataGridHasFocus;

    /// <summary>
    /// DataGrid has focus
    /// </summary>
    public bool DataGridHasFocus
    {
      get => _dataGridHasFocus;
      set
      {
        _dataGridHasFocus = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FileManagerViewModel()
    {
      _xmlFileManagerController = new XmlFileManagerController();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

      DataGridHasFocus = true;
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    private ICommand _openCommand;

    /// <summary>
    /// Open command
    /// </summary>
    public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(p => CanExecuteOpenCommand(), p => ExecuteOpenCommand()));

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((Window) p)));

    private IAsyncCommand _saveCommand;

    /// <summary>
    /// Save command
    /// </summary>
    public IAsyncCommand SaveCommand => _saveCommand ?? (_saveCommand = AsyncCommand.Create(p => SelectedItem != null, ExecuteSaveCommandAsync));

    private IAsyncCommand _deleteTailDataCommand;

    /// <summary>
    /// Delete <see cref="TailData"/> from FileManager
    /// </summary>
    public IAsyncCommand DeleteTailDataCommand => _deleteTailDataCommand ?? (_deleteTailDataCommand = AsyncCommand.Create(p => SelectedItem != null, ExecuteDeleteCommandAsync));

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => SelectedItem != null, p => ExecuteUndoCommand()));

    private ICommand _addTailDataCommand;

    /// <summary>
    /// Add <see cref="TailData"/> command
    /// </summary>
    public ICommand AddTailDataCommand => _addTailDataCommand ?? (_addTailDataCommand = new RelayCommand(p => ExecuteAddTailDataCommand()));

    private ICommand _openFileCommand;

    /// <summary>
    /// Open file command
    /// </summary>
    public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(p => ExecuteOpenFileCommand()));

    #endregion

    #region Command functions

    private void ExecuteOpenFileCommand()
    {
      if ( !FileOpenDialog.OpenDialog("All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle, out string fileName) )
        return;

      SelectedItem.FileName = fileName;
    }

    private void ExecuteAddTailDataCommand()
    {
      SelectedItem = new TailData();
      FileManagerCollection.Add(SelectedItem);
    }

    private void ExecuteUndoCommand()
    {
      if ( _mementoTailData == null || SelectedItem == null )
        return;

      MouseService.SetBusyState();
      SelectedItem.RestoreFromMemento(_mementoTailData);
    }

    private async Task ExecuteDeleteCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      MouseService.SetBusyState();

      FileManagerCollection.Remove(SelectedItem);
      await _xmlFileManagerController.DeleteTailDataByIdFromXmlFileAsync(_cts.Token, SelectedItem.Id.ToString()).ConfigureAwait(false);
    }

    private async Task ExecuteSaveCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      MouseService.SetBusyState();
      await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, SelectedItem).ConfigureAwait(false);
    }

    private async Task<ObservableCollection<TailData>> ExecuteLoadedCommandAsync()
    {
      try
      {
        FileManagerCollection = await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
      }
      catch
      {
        // Nothing
      }
      return FileManagerCollection;
    }

    private bool CanExecuteOpenCommand() => !string.IsNullOrWhiteSpace(SelectedItem?.FileName) && File.Exists(SelectedItem.FileName);

    private void ExecuteOpenCommand()
    {

    }

    private void ExecuteCloseCommand(Window window)
    {
      MouseService.SetBusyState();

      if ( _mementoTailData != null )
        SelectedItem.RestoreFromMemento(_mementoTailData);

      _mementoTailData = null;
      _cts.Cancel();
      window?.Close();
    }

    #endregion
  }
}
