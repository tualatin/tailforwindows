using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
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

    private readonly CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;
    private readonly CollectionView _collectionView;

    #region Properties

    private ObservableCollection<TailData> _fileManagerCollection;

    /// <summary>
    /// FileManager collection
    /// </summary>
    public ObservableCollection<TailData> FileManagerCollection
    {
      get => _fileManagerCollection;
      set
      {
        if ( value == _fileManagerCollection )
          return;

        _fileManagerCollection = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<TailData> _selectedItems;

    /// <summary>
    /// SelectedItems
    /// </summary>
    public ObservableCollection<TailData> SelectedItems
    {
      get => _selectedItems;
      set
      {
        if ( value == _selectedItems )
          return;

        _selectedItems = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<string> _categories;

    /// <summary>
    /// Categories
    /// </summary>
    public ObservableCollection<string> Categories
    {
      get => _categories;
      set
      {
        if ( value == _categories )
          return;

        _categories = value;
        OnPropertyChanged();
      }
    }

    private string _selectedCategory;

    /// <summary>
    /// Selected category
    /// </summary>
    public string SelectedCategory
    {
      get => _selectedCategory;
      set
      {
        if ( Equals(value, _selectedCategory) )
          return;

        _selectedCategory = value;
        OnPropertyChanged();
      }
    }

    private TailData _selectedItem;

    /// <summary>
    /// Selected item
    /// </summary>
    public TailData SelectedItem
    {
      get => _selectedItem;
      set
      {
        if ( value == _selectedItem )
          return;

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

      Categories = new ObservableCollection<string>();
      _collectionView = (CollectionView) CollectionViewSource.GetDefaultView(FileManagerCollection);

      ((AsyncCommand<object>) DeleteTailDataCommand).PropertyChanged += OnDeleteTailDataPropertyChanged;
      ((AsyncCommand<object>) SaveCommand).PropertyChanged += OnSaveTailDataPropertyChanged;
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnSaveTailDataPropertyChanged;

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
    public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(p => CanExecuteOpenCommand(), p => ExecuteOpenCommand((Window) p)));

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((Window) p)));

    private IAsyncCommand _saveCommand;

    /// <summary>
    /// Save command
    /// </summary>
    public IAsyncCommand SaveCommand => _saveCommand ?? (_saveCommand = AsyncCommand.Create(p => CanExecuteSaveCommand(), ExecuteSaveCommandAsync));

    private IAsyncCommand _deleteTailDataCommand;

    /// <summary>
    /// Delete <see cref="TailData"/> from FileManager
    /// </summary>
    public IAsyncCommand DeleteTailDataCommand => _deleteTailDataCommand ?? (_deleteTailDataCommand = AsyncCommand.Create(p => SelectedItem != null, ExecuteDeleteCommandAsync));

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

    private ICommand _dataGridMouseDoubleClickCommand;

    /// <summary>
    /// MouseDoubleClick command
    /// </summary>
    public ICommand DataGridMouseDoubleClickCommand => _dataGridMouseDoubleClickCommand ?? (_dataGridMouseDoubleClickCommand = new RelayCommand(p => SelectedItem != null, p => ExecuteMouseDoubleClickCommmand((Window) p)));

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndo(), p => ExecuteUndoCommand()));

    #endregion

    #region Command functions

    private bool CanExecuteUndo()
    {
      var result = FileManagerCollection?.Where(p => p.CanUndo).ToList();

      return result?.Count > 0;
    }

    private void ExecuteUndoCommand()
    {

    }

    private void ExecuteMouseDoubleClickCommmand(Window window) => OpenSelectedItem(window);

    private void ExecuteOpenFileCommand()
    {
      if ( !FileOpenDialog.OpenDialog("All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle, out string fileName) )
        return;

      SelectedItem.FileName = fileName;
    }

    private void ExecuteAddTailDataCommand() => FileManagerCollection.Add(new TailData());

    private async Task ExecuteDeleteCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      MouseService.SetBusyState();

      if ( SelectedItem.IsLoadedByXml )
        await _xmlFileManagerController.DeleteTailDataByIdFromXmlFileAsync(_cts.Token, SelectedItem.Id.ToString()).ConfigureAwait(false);
    }

    private bool CanExecuteSaveCommand()
    {
      if ( FileManagerCollection == null || FileManagerCollection.Count == 0 )
        return false;

      var errors = FileManagerCollection.Where(p => p["Description"] != null || p["FileName"] != null).ToList();
      bool undo = CanExecuteUndo();

      return errors.Count <= 0 && undo;
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();

      foreach ( var tailData in FileManagerCollection )
      {
        if ( tailData.IsLoadedByXml )
        {
          await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, tailData).ConfigureAwait(false);
        }
        else
        {
          tailData.IsLoadedByXml = true;
          await _xmlFileManagerController.AddTailDataToXmlFileAsync(_cts.Token, tailData).ConfigureAwait(false);
        }
      }

      _categories = await _xmlFileManagerController.GetCategoriesFromXmlFileAsync(FileManagerCollection).ConfigureAwait(false);
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      try
      {
        _fileManagerCollection = await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
        _categories = await _xmlFileManagerController.GetCategoriesFromXmlFileAsync(_fileManagerCollection).ConfigureAwait(false);

        if ( _fileManagerCollection != null )
        {
          foreach ( var tailData in _fileManagerCollection )
          {
            tailData.Clear();
          }
        }
      }
      catch
      {
        // Nothing
      }
    }

    private bool CanExecuteOpenCommand() => !string.IsNullOrWhiteSpace(SelectedItem?.FileName) && File.Exists(SelectedItem.FileName);

    private void ExecuteOpenCommand(Window window)
    {
      if ( SelectedItem == null )
        return;

      OpenSelectedItem(window);
    }

    private void ExecuteCloseCommand(Window window)
    {
      _cts.Cancel();
      window?.Close();
    }

    #endregion

    private void OpenSelectedItem(Window window)
    {
      // Is this file already open?
      var result = BusinessHelper.GetTabItemList().Where(p => ((LogWindowControl) p.Content).CurrenTailData.Id == SelectedItem.Id).ToList();

      if ( result.Count > 0 )
      {
        EnvironmentContainer.ShowInformationMessageBox(Application.Current.TryFindResource("FileManagerFileAlreadyOpen").ToString());
        return;
      }

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataMessage(this, SelectedItem));
      ExecuteCloseCommand(window);
    }

    private void OnDeleteTailDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( SelectedItem == null )
        return;

      if ( FileManagerCollection.Contains(SelectedItem) )
        FileManagerCollection.Remove(SelectedItem);

      OnPropertyChanged(nameof(FileManagerCollection));
    }

    private void OnSaveTailDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( _fileManagerCollection == null || _fileManagerCollection.Count == 0 )
        FileManagerCollection = new ObservableCollection<TailData> { new TailData() };

      OnPropertyChanged(nameof(Categories));
      OnPropertyChanged(nameof(FileManagerCollection));
    }
  }
}
