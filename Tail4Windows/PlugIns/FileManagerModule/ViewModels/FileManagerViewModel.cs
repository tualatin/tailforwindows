using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.FontChooserModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.PatternModule;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels
{
  /// <summary>
  /// FileManager view model
  /// </summary>
  public class FileManagerViewModel : NotifyMaster, IFileDragDropTarget, IViewModelBase
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileManagerViewModel));

    private CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;
    private readonly List<Predicate<TailData>> _criteria = new List<Predicate<TailData>>();

    #region Properties

    /// <summary>
    /// FileManager view
    /// </summary>
    public ListCollectionView FileManagerView
    {
      get;
      set;
    }

    private ObservableCollection<TailData> _fileManagerCollection;

    /// <summary>
    /// FileManager collection
    /// </summary>
    private ObservableCollection<TailData> FileManagerCollection
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

    /// <summary>
    /// Selected item
    /// </summary>
    private TailData SelectedItem
    {
      get => FileManagerView?.CurrentItem as TailData;
      set
      {
        FileManagerView.MoveCurrentTo(value);
        OnPropertyChanged();
      }
    }

    private bool _filterHasFocus;

    /// <summary>
    /// Filter has focus
    /// </summary>
    public bool FilterHasFocus
    {
      get => _filterHasFocus;
      set
      {
        _filterHasFocus = value;
        OnPropertyChanged();
      }
    }

    private string _filterText;

    /// <summary>
    /// Current filter text
    /// </summary>
    public string FilterText
    {
      get => _filterText;
      set
      {
        if ( Equals(value, _filterText) )
          return;

        _filterText = value;
        OnPropertyChanged();

        _criteria.Clear();

        if ( string.IsNullOrWhiteSpace(_filterText) )
        {
          FileManagerView.Filter = DynamicFilter;
          return;
        }

        _criteria.Add(p => !string.IsNullOrEmpty(p.Category) && !string.IsNullOrEmpty(p.Description) &&
                           (p.Category.ToLower().StartsWith(_filterText) || p.Description.ToLower().StartsWith(_filterText)));

        FileManagerView.Filter = DynamicFilter;

        if ( SelectedItem == null )
          return;

        // Bring the current TailData back into view in case it moved
        TailData currentTailData = SelectedItem;
        FileManagerView.MoveCurrentToFirst();
        FileManagerView.MoveCurrentTo(currentTailData);
      }
    }

    /// <summary>
    /// Parent Guid
    /// </summary>
    public Guid ParentGuid
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FileManagerViewModel()
    {
      _xmlFileManagerController = new XmlFileManagerController();
      Categories = new ObservableCollection<string>();

      SetCancellationTokenSource();

      ((AsyncCommand<object>) DeleteTailDataCommand).PropertyChanged += OnDeleteTailDataPropertyChanged;
      ((AsyncCommand<object>) SaveCommand).PropertyChanged += OnSaveTailDataPropertyChanged;
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnSaveTailDataPropertyChanged;
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

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
    public ICommand DataGridMouseDoubleClickCommand => _dataGridMouseDoubleClickCommand ?? (_dataGridMouseDoubleClickCommand = new RelayCommand(p => SelectedItem != null, ExecuteMouseDoubleClickCommmand));

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndo(), p => ExecuteUndoCommand()));

    private ICommand _groupByClickCommand;

    /// <summary>
    /// Group by click command
    /// </summary>
    public ICommand GroupByClickCommand => _groupByClickCommand ?? (_groupByClickCommand = new RelayCommand(p => FileManagerCollection != null && FileManagerCollection.Count >= 2,
                                             p => ExecuteGroupByClickCommand()));

    private ICommand _fontCommand;

    /// <summary>
    /// Font command
    /// </summary>
    public ICommand FontCommand => _fontCommand ?? (_fontCommand = new RelayCommand(p => SelectedItem != null, p => ExecuteFontCommand((Window) p)));

    private ICommand _filterCommand;

    /// <summary>
    /// Filter command
    /// </summary>
    public ICommand FilterCommand => _filterCommand ?? (_filterCommand = new RelayCommand(p => SelectedItem != null, p => ExecuteFilterCommand((Window) p)));

    private ICommand _previewDragEnterCommand;

    /// <summary>
    /// Preview drag enter command
    /// </summary>
    public ICommand PreviewDragEnterCommand => _previewDragEnterCommand ?? (_previewDragEnterCommand = new RelayCommand(ExecutePreviewDragEnterCommand));

    private ICommand _patternControlCommand;

    /// <summary>
    /// PatternControl command
    /// </summary>
    public ICommand PatternControlCommand => _patternControlCommand ?? (_patternControlCommand = new RelayCommand(p => ExecutePatternControlCommand((Window) p)));

    #endregion

    #region Command functions

    private void ExecutePatternControlCommand(Window window)
    {
      var patternControl = new PatternControl
      {
        Owner = window,
        CurrentTailData = SelectedItem
      };
      patternControl.ShowDialog();
    }

    private void ExecutePreviewDragEnterCommand(object parameter)
    {
      if ( !(parameter is DragEventArgs e) )
        return;

      e.Handled = true;
      e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
    }

    private void ExecuteFilterCommand(Window window)
    {
      var filterManager = new FilterManager
      {
        Owner = window
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFilterDataFromTailDataMessage(this, SelectedItem));
      filterManager.ShowDialog();

      OnPropertyChanged(nameof(SelectedItem));
    }

    private void ExecuteFontCommand(Window window)
    {
      if ( SelectedItem == null )
        return;

      var fontManager = new FontChooseDialog
      {
        Owner = window,
        SelectedFont = new FontInfo(SelectedItem.FontType.FontFamily, SelectedItem.FontType.FontSize, SelectedItem.FontType.FontStyle,
          SelectedItem.FontType.FontWeight, SelectedItem.FontType.FontStretch)
      };

      if ( fontManager.ShowDialog() == true )
        SelectedItem.FontType = fontManager.SelectedFont.FontType;
    }

    private void ExecuteGroupByClickCommand() => SetFileManagerViewGrouping();

    private bool CanExecuteUndo()
    {
      if ( SelectedItem == null )
        return false;

      return SelectedItem.FindSettings.CanUndo || SelectedItem.CanUndo;
    }

    private void ExecuteUndoCommand() => SelectedItem?.Undo();

    private void ExecuteMouseDoubleClickCommmand(object param)
    {
      if ( !(param is object[] o) )
        return;

      if ( !(o.First() is MouseButtonEventArgs e) || !(o.Last() is Window) )
        return;

      var dep = e.OriginalSource as DependencyObject;

      while ( dep != null && !(dep is DataGridCell) )
      {
        dep = VisualTreeHelper.GetParent(dep);
      }

      if ( dep == null )
        return;

      var cell = dep as DataGridCell;

      if ( !Equals(cell.Column.Header, Application.Current.TryFindResource("FileManagerDataGridNo").ToString()) )
        return;

      OpenSelectedItem(o.Last() as Window);
    }

    private void ExecuteOpenFileCommand()
    {
      if ( !InteractionService.OpenFileDialog(out string fileName, "All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle) )
        return;

      MouseService.SetBusyState();

      SelectedItem.FileName = fileName;
      SelectedItem.FileEncoding = EncodingDetector.GetEncodingAsync(SelectedItem.FileName).Result;
    }

    private void ExecuteAddTailDataCommand()
    {
      var newItem = new TailData();
      FileManagerCollection.Add(newItem);
      SelectedItem = FileManagerCollection.Last();

      OnPropertyChanged(nameof(FileManagerView));
    }

    private async Task ExecuteDeleteCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      if ( !FileManagerCollection.Contains(SelectedItem) )
        return;

      bool errors = SelectedItem["Description"] != null || SelectedItem["FileName"] != null;

      if ( !errors || SelectedItem.IsLoadedByXml )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerDeleteItemQuestion").ToString()) == MessageBoxResult.No )
          return;
      }

      MouseService.SetBusyState();

      string id = SelectedItem.Id.ToString();
      await _xmlFileManagerController.DeleteTailDataByIdFromXmlFileAsync(_cts.Token, id).ConfigureAwait(false);
    }

    private bool CanExecuteSaveCommand()
    {
      if ( FileManagerCollection == null || FileManagerCollection.Count == 0 )
        return false;

      var errors = GetErrors();
      bool undo = CanExecuteUndo();
      var unsavedItems = new List<TailData>();

      if ( errors.Count == 0 )
        unsavedItems = FileManagerCollection.Where(p => !p.IsLoadedByXml).ToList();

      if ( errors.Count > 0 )
        return false;

      return unsavedItems.Count > 0 || undo;
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

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

      CommitChanges();
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      try
      {
        _fileManagerCollection = await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
        _categories = await _xmlFileManagerController.GetCategoriesFromXmlFileAsync(_fileManagerCollection).ConfigureAwait(false);

        CommitChanges();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
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
      var errors = GetErrors();

      if ( errors.Count > 0 )
      {
        foreach ( var filterData in errors )
        {
          FileManagerCollection.Remove(filterData);
        }

        OnPropertyChanged(nameof(FileManagerView));
      }

      var unsavedItems = FileManagerCollection.Where(p => p.CanUndo || p.FindSettings.CanUndo).ToList();

      if ( unsavedItems.Count > 0 )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerCloseUnsaveItem").ToString()) == MessageBoxResult.Yes )
        {
          ExecuteSaveCommandAsync().GetAwaiter().GetResult();
        }
        else
        {
          foreach ( var item in unsavedItems )
          {
            FileManagerCollection.Remove(item);
          }
        }
      }

      _cts.Cancel();
      window?.Close();
    }

    #endregion

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }

    private void OpenSelectedItem(Window window)
    {
      // Is this file already open?
      var result = BusinessHelper.GetTabItemList().Where(p => ((LogWindowControl) p.Content).CurrentTailData.Id == SelectedItem.Id).ToList();

      if ( result.Count > 0 )
      {
        InteractionService.ShowInformationMessageBox(Application.Current.TryFindResource("FileManagerFileAlreadyOpen").ToString());
        return;
      }

      if ( string.IsNullOrWhiteSpace(SelectedItem?.FileName) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataMessage(this, SelectedItem, ParentGuid, false));
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
      OnPropertyChanged(nameof(FileManagerView));
    }

    private void OnSaveTailDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( _fileManagerCollection == null || _fileManagerCollection.Count == 0 )
      {
        FileManagerCollection = new ObservableCollection<TailData> { new TailData() };
        FileManagerCollection.First().CommitChanges();
      }

      FilterHasFocus = false;
      FileManagerView = (ListCollectionView) new CollectionViewSource { Source = FileManagerCollection }.View;
      FileManagerView.CustomSort = new TailDataComparer();
      FileManagerView.Filter = DynamicFilter;

      if ( FileManagerCollection.Count >= 2 )
        SetFileManagerViewGrouping();

      TailManagerCollectionViewHolder.Cv = FileManagerView;
      SelectedItem = FileManagerCollection.First();

      OnPropertyChanged(nameof(FileManagerView));
      OnPropertyChanged(nameof(Categories));
      OnPropertyChanged(nameof(FileManagerCollection));

      FilterHasFocus = WaitAsync().Result;
    }

    private void SetFileManagerViewGrouping()
    {
      FileManagerView.GroupDescriptions.Clear();

      if ( SettingsHelperController.CurrentSettings.GroupByCategory )
        FileManagerView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
    }

    private async Task<bool> WaitAsync()
    {
      // Wait some ms to set the correct focus
      await Task.Delay(TimeSpan.FromMilliseconds(25)).ConfigureAwait(false);
      return true;
    }

    private void CommitChanges()
    {
      Parallel.ForEach(_fileManagerCollection, f =>
      {
        f.CommitChanges();
        f.FindSettings.CommitChanges();

        Parallel.ForEach(f.ListOfFilter, p =>
        {
          p.FindSettingsData.CommitChanges();
        });
      });
    }

    private List<TailData> GetErrors()
    {
      var errors = FileManagerCollection.Where(p => p["Description"] != null || p["FileName"] != null).ToList();
      return errors;
    }

    private bool DynamicFilter(object item)
    {
      TailData t = item as TailData;

      if ( _criteria.Count == 0 )
        return true;

      bool result = _criteria.TrueForAll(p => p(t));

      return result;
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

        SelectedItem.FileName = fileName;
        OnPropertyChanged(nameof(SelectedItem));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
  }
}
