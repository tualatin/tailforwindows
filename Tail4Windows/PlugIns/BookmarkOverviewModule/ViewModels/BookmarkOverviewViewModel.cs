using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.BookmarkEngine.Interfaces;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Business.ExportEngine;
using Org.Vs.TailForWin.Business.ExportEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.BookmarkCommentModule;
using Org.Vs.TailForWin.PlugIns.ExportFormatModule;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.FloatWindow;
using Org.Vs.TailForWin.UI.UserControls;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.PlugIns.BookmarkOverviewModule.ViewModels
{
  /// <summary>
  /// Bookmark overview module view model
  /// </summary>
  public class BookmarkOverviewViewModel : NotifyMaster, IBookmarkOverviewViewModel
  {
    private readonly ISettingsDbController _dbController;
    private readonly List<Predicate<LogEntry>> _criteria = new List<Predicate<LogEntry>>();
    private readonly IDataExport<LogEntry> _dataExport;

    #region Properties

    private double _topPosition;

    /// <summary>
    /// Top position
    /// </summary>
    public double TopPosition
    {
      get => _topPosition;
      set
      {
        _topPosition = value;
        OnPropertyChanged();
      }
    }

    private double _leftPosition;

    /// <summary>
    /// Left position
    /// </summary>
    public double LeftPosition
    {
      get => _leftPosition;
      set
      {
        _leftPosition = value;
        OnPropertyChanged();
      }
    }

    private double _windowHeight;

    /// <summary>
    /// Window height
    /// </summary>
    public double WindowHeight
    {
      get => _windowHeight;
      set
      {
        _windowHeight = value;
        OnPropertyChanged();
      }
    }

    private double _windowWidth;

    /// <summary>
    /// Window width
    /// </summary>
    public double WindowWidth
    {
      get => _windowWidth;
      set
      {
        _windowWidth = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Bookmark view
    /// </summary>
    public ListCollectionView BookmarkCollectionView
    {
      get;
      set;
    }

    private IList _selectedItems;

    /// <summary>
    /// SelectedItems
    /// </summary>
    public IList SelectedItems
    {
      get => _selectedItems;
      set
      {
        if ( Equals(value, _selectedItems) )
          return;

        _selectedItems = value;
        OnPropertyChanged();
      }
    }

    private LogEntry _selectedItem;

    /// <summary>
    /// Selected item
    /// </summary>
    public LogEntry SelectedItem
    {
      get => _selectedItem;
      set
      {
        if ( Equals(value, _selectedItem) )
          return;

        _selectedItem = value;
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
          BookmarkCollectionView.Filter = DynamicFilter;
          return;
        }

        _criteria.Add(p => !string.IsNullOrWhiteSpace(p.BookmarkToolTip) && p.BookmarkToolTip.ToLower().Contains(_filterText));
        BookmarkCollectionView.Filter = DynamicFilter;
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

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BookmarkOverviewViewModel()
    {
      _dbController = SettingsDbController.Instance;
      _dataExport = new ExportBookmarkDataSource();
      EnvironmentContainer.Instance.BookmarkManager.OnBookmarkDataSourceChanged += OnBookmarkManagerBookmarkDataSourceChanged;
    }

    #region Commands

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    private ICommand _closingCommand;

    /// <summary>
    /// Closing command
    /// </summary>
    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand(p => ExecuteClosingCommand()));

    private IAsyncCommand _exportCommand;

    /// <summary>
    /// Export command
    /// </summary>
    public IAsyncCommand ExportCommand => _exportCommand ?? (_exportCommand = AsyncCommand.Create(p => CanExecuteExportCommand(), (p, t) => ExecuteExportCommandAsync(p)));

    private ICommand _bookmarkOverviewMouseDoubleClickCommand;

    /// <summary>
    /// BookmarkOverview mouse double click command
    /// </summary>
    public ICommand BookmarkOverviewMouseDoubleClickCommand => _bookmarkOverviewMouseDoubleClickCommand ??
                                                               (_bookmarkOverviewMouseDoubleClickCommand = new RelayCommand(ExecuteMouseDoubleClickCommand));

    private ICommand _removeBookmarksCommand;

    /// <summary>
    /// Remove bookmarks command
    /// </summary>
    public ICommand RemoveBookmarksCommand => _removeBookmarksCommand ?? (_removeBookmarksCommand = new RelayCommand(p => CanExecuteRemoveBookmarksCommand(),
                                                p => ExecuteRemoveBookmarksCommand()));

    private ICommand _addBookmarkCommentCommand;

    /// <summary>
    /// Add bookmark comment command
    /// </summary>
    public ICommand AddBookmarkCommentCommand => _addBookmarkCommentCommand ?? (_addBookmarkCommentCommand = new RelayCommand(p => CanExecuteRemoveBookmarksCommand(),
                                                   ExecuteAddBookmarkCommentCommand));

    #endregion

    #region Command functions

    private void ExecuteAddBookmarkCommentCommand(object param)
    {
      if ( !(param is VsDataGrid vsDataGrid) )
        return;

      VsFloatingWindow bookmarkOverviewWindow = vsDataGrid.Ancestors().OfType<VsFloatingWindow>().FirstOrDefault(p => p.Name == "BookmarkOverviewWindow");
      var addBookmarkCommentPopup = new AddBookmarkComment
      {
        Owner = bookmarkOverviewWindow,
        Comment = (SelectedItems[0] as LogEntry)?.BookmarkToolTip
      };
      addBookmarkCommentPopup.ShowDialog();

      foreach ( LogEntry item in SelectedItems )
      {
        item.BookmarkToolTip = addBookmarkCommentPopup.Comment;
      }
    }

    private bool CanExecuteRemoveBookmarksCommand() => SelectedItems != null && SelectedItems.Count > 0;

    private void ExecuteRemoveBookmarksCommand()
    {
      if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("BookmarkOverviewQDeleteBookmark").ToString()) == MessageBoxResult.No )
        return;

      for ( int i = SelectedItems.Count - 1; i >= 0; i-- )
      {
        if ( !(SelectedItems[i] is LogEntry logEntry) )
          continue;

        logEntry.BookmarkPoint = null;
        logEntry.BookmarkToolTip = string.Empty;

        EnvironmentContainer.Instance.BookmarkManager.RemoveFromBookmarkDataSource(logEntry);
      }
    }

    private void ExecuteClosingCommand()
    {
      SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX = LeftPosition;
      SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY = TopPosition;

      SettingsHelperController.CurrentSettings.BookmarkOverviewHeight = WindowHeight;
      SettingsHelperController.CurrentSettings.BookmarkOverviewWidth = WindowWidth;

      _dbController.UpdateBookmarkOverviewDbSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token).SafeAwait();
    }

    private void ExecuteMouseDoubleClickCommand(object param)
    {
      if ( !(param is MouseButtonEventArgs e) )
        return;

      if ( !(e.Source is VsDataGrid dg) )
        return;

      if ( !(dg.CurrentItem is LogEntry selectedItem) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new JumpToSelectedLogEntryMessage(EnvironmentContainer.Instance.BookmarkManager.GetCurrentWindowId(),
        selectedItem));
    }

    private bool CanExecuteExportCommand() => BookmarkCollectionView != null && BookmarkCollectionView.Count > 0;

    private async Task ExecuteExportCommandAsync(object param)
    {
      if ( !(param is BookmarkOverview window) )
        return;

      var exportFormatSelector = new ExportFormatSelector
      {
        Owner = window
      };
      var dialogResult = exportFormatSelector.ShowDialog();

      if ( dialogResult != null && !(bool) dialogResult )
        return;

      string fileName = string.Empty;
      EExportFormat exportFormat = exportFormatSelector.ExportFormat;
      BookmarkExportData exportData = new BookmarkExportData();

      if ( exportFormat == EExportFormat.None )
        return;

      if ( !OpenSaveDialog(ref fileName, Application.Current.TryFindResource("ExportFormatSelectorExportAllFiles").ToString()) )
        return;

      string folder = Path.GetDirectoryName(fileName);
      fileName = Path.GetFileNameWithoutExtension(fileName);

      if ( exportFormat.HasFlag(EExportFormat.Csv) )
      {
        MouseService.SetBusyState();
        exportData.Success = await _dataExport.ExportAsCsvAsync(EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource, $@"{folder}\{fileName}.csv").ConfigureAwait(false);
        exportData.CsvExport = true;
      }

      if ( exportFormat.HasFlag(EExportFormat.Excel) )
      {
        MouseService.SetBusyState();
        var result = await _dataExport.ExportAsExcelAsync(EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource, $@"{folder}\{fileName}.xlsx").ConfigureAwait(false);

        if ( exportData.CsvExport )
          exportData.Success &= result;
        else
          exportData.Success = result;

        exportData.ExcelExport = true;
      }

      if ( exportFormat.HasFlag(EExportFormat.OpenDocument) )
      {
        MouseService.SetBusyState();
        var result = await _dataExport.ExportAsOpenDocumentAsync(EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource, $@"{folder}\{fileName}.").ConfigureAwait(false);

        if ( exportData.CsvExport || exportData.ExcelExport )
          exportData.Success &= result;
        else
          exportData.Success = result;

        exportData.OpenDocumentExport = true;
      }

      if ( exportData.CsvExport || exportData.ExcelExport || exportData.OpenDocumentExport )
        ShowExportResult(exportData.Success);
    }

    private void ShowExportResult(bool result)
    {
      if ( result )
        InteractionService.ShowInformationMessageBox(Application.Current.TryFindResource("BookmarkOverviewExportSuccess").ToString());
      else
        InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("BookmarkOverviewExportFailed").ToString());
    }

    private bool OpenSaveDialog(ref string fileName, string filter) =>
      InteractionService.OpenSaveDialog(ref fileName, filter, Application.Current.TryFindResource("BookmarkOverviewSaveDialogTitle").ToString());

    private void ExecuteLoadedCommand()
    {
      MoveIntoView();

      TopPosition = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY;
      LeftPosition = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX;

      WindowHeight = SettingsHelperController.CurrentSettings.BookmarkOverviewHeight;
      WindowWidth = SettingsHelperController.CurrentSettings.BookmarkOverviewWidth;
    }

    #endregion

    /// <summary>
    /// Setup Bookmark collection view
    /// </summary>
    public void SetupBookmarkCollectionView()
    {
      FilterHasFocus = false;

      if ( EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource == null )
      {
        FilterHasFocus = true;
        return;
      }

      BookmarkCollectionView = (ListCollectionView) new CollectionViewSource { Source = EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource }.View;
      BookmarkCollectionView.CustomSort = new LogEntryComparer();
      BookmarkCollectionView.Filter = DynamicFilter;

      SelectedItem = BookmarkCollectionView.Count == 0 ? null : EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource.First();
      FilterHasFocus = true;
      OnPropertyChanged(nameof(BookmarkCollectionView));
    }

    private bool DynamicFilter(object item)
    {
      var t = item as LogEntry;

      if ( _criteria.Count == 0 )
        return true;

      bool result = _criteria.TrueForAll(p => p(t));

      return result;
    }

    private void OnBookmarkManagerBookmarkDataSourceChanged(object sender, EventArgs e)
    {
      if ( !(sender is IBookmarkManager) )
        return;

      SetupBookmarkCollectionView();
    }

    private void MoveIntoView()
    {
      double posX = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX;
      double posY = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY;

      UiHelper.MoveIntoView(Application.Current.TryFindResource("BookmarkOverviewTitle").ToString(), ref posX, ref posY, SettingsHelperController.CurrentSettings.BookmarkOverviewWidth,
        SettingsHelperController.CurrentSettings.BookmarkOverviewHeight);

      SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX = posX;
      SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY = posY;
    }
  }
}
