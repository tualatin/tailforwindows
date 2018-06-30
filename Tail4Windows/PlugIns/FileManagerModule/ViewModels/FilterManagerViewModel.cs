using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.FontChooserModule;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels
{
  /// <summary>
  /// FilterManager view model
  /// </summary>
  public class FilterManagerViewModel : NotifyMaster, IFilterManagerViewModel
  {
    private CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;
    private bool _filterAdded;

    #region Properties

    private TailData _currenTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    private TailData CurrentTailData
    {
      get => _currenTailData;
      set
      {
        _currenTailData = value;

        if ( _currenTailData == null )
          SaveButtonVisibility = Visibility.Collapsed;
        else
          SaveButtonVisibility = CurrentTailData.IsLoadedByXml ? Visibility.Visible : Visibility.Collapsed;

        OnPropertyChanged();
      }
    }

    /// <summary>
    /// FileManager view
    /// </summary>
    public ListCollectionView FilterManagerView
    {
      get;
      set;
    }

    private ObservableCollection<FilterData> FilterManagerCollection
    {
      get => CurrentTailData?.ListOfFilter;
      set
      {
        if ( value == CurrentTailData.ListOfFilter )
          return;

        CurrentTailData.ListOfFilter = value;
        OnPropertyChanged();
      }
    }

    private FilterData SelectedItem
    {
      get => FilterManagerView?.CurrentItem as FilterData;
      set
      {
        FilterManagerView?.MoveCurrentTo(value);
        OnPropertyChanged();
      }
    }

    private Visibility _saveButtonVisibility;

    /// <summary>
    /// SaveButtonVisibility
    /// </summary>
    public Visibility SaveButtonVisibility
    {
      get => _saveButtonVisibility;
      set
      {
        _saveButtonVisibility = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FilterManagerViewModel()
    {
      _xmlFileManagerController = new XmlFileManagerController();

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFilterDataFromTailDataMessage>(OnOpenTailData);
      SetCancellationTokenSource();
    }

    private void OnOpenTailData(OpenFilterDataFromTailDataMessage args)
    {
      if ( args == null )
        return;

      CurrentTailData = args.TailData;

      if ( CurrentTailData.ListOfFilter.Count == 0 )
        CurrentTailData.ListOfFilter.Add(new FilterData());

      foreach ( var item in CurrentTailData.ListOfFilter )
      {
        item.CommitChanges();
      }

      if ( string.IsNullOrWhiteSpace(args.FilterPattern) )
        return;

      var errors = GetFilterErrors();

      if ( CurrentTailData.ListOfFilter.Count >= 1 && errors.Count == 0 )
        CurrentTailData.ListOfFilter.Add(new FilterData { Filter = args.FilterPattern });
      else
        CurrentTailData.ListOfFilter.First().Filter = args.FilterPattern;

      _filterAdded = true;
    }

    #region Commands

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

    private ICommand _addFilterDataCommand;

    /// <summary>
    /// Add <see cref="FilterData"/> command
    /// </summary>
    public ICommand AddFilterDataCommand => _addFilterDataCommand ?? (_addFilterDataCommand = new RelayCommand(p => ExecuteAddFilterDataCommand()));

    private IAsyncCommand _deleteFilterDataCommand;

    /// <summary>
    /// Delete <see cref="FilterData"/> from FileManager
    /// </summary>
    public IAsyncCommand DeleteFilterDataCommand => _deleteFilterDataCommand ?? (_deleteFilterDataCommand = AsyncCommand.Create(p => SelectedItem != null, ExecuteDeleteCommandAsync));

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndo(), p => ExecuteUndoCommand()));

    private ICommand _fontCommand;

    /// <summary>
    /// Font command
    /// </summary>
    public ICommand FontCommand => _fontCommand ?? (_fontCommand = new RelayCommand(p => SelectedItem != null, p => ExecuteFontCommand((Window) p)));

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    #endregion

    #region Command functions

    private void ExecuteLoadedCommand()
    {
      FilterManagerView = (ListCollectionView) new CollectionViewSource { Source = FilterManagerCollection }.View;
      FilterManagerCollectionViewHolder.Cv = FilterManagerView;

      OnPropertyChanged(nameof(FilterManagerCollection));
      OnPropertyChanged(nameof(FilterManagerView));

      if ( FilterManagerView.Count == 0 )
        return;

      SelectedItem = !_filterAdded ? FilterManagerCollection.First() : FilterManagerCollection.Last();
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

    private bool CanExecuteUndo()
    {
      if ( SelectedItem?.FindSettingsData == null )
        return false;

      return SelectedItem.CanUndo || SelectedItem.FindSettingsData.CanRedo;
    }

    private void ExecuteUndoCommand()
    {
      SelectedItem?.Undo();
      SelectedItem?.FindSettingsData?.Undo();
    }

    private void ExecuteAddFilterDataCommand()
    {
      var newItem = new FilterData();
      newItem.CommitChanges();
      newItem.FindSettingsData.CommitChanges();

      FilterManagerCollection.Add(newItem);
      SelectedItem = FilterManagerCollection.Last();

      OnPropertyChanged(nameof(FilterManagerView));
    }

    private async Task ExecuteDeleteCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      if ( !FilterManagerCollection.Contains(SelectedItem) )
        return;

      bool error = SelectedItem["Description"] != null || SelectedItem["Filter"] != null || SelectedItem["FilterSource"] != null || SelectedItem["IsHighlight"] != null;

      if ( !error )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerDeleteItemQuestion").ToString()) == MessageBoxResult.No )
          return;
      }

      MouseService.SetBusyState();

      FilterManagerCollection.Remove(SelectedItem);

      if ( CurrentTailData.IsLoadedByXml )
        await ExecuteSaveCommandAsync().ConfigureAwait(false);

      OnPropertyChanged(nameof(FilterManagerView));
    }

    private bool CanExecuteSaveCommand()
    {
      if ( CurrentTailData == null )
        return false;

      var errors = GetFilterErrors();
      bool undo = CanExecuteUndo();

      return errors.Count <= 0 && undo && CurrentTailData.IsLoadedByXml && FilterManagerCollection != null && FilterManagerCollection.Count > 0;
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
      await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, CurrentTailData).ConfigureAwait(false);

      foreach ( var item in FilterManagerCollection )
      {
        item.CommitChanges();
        item.FindSettingsData.CommitChanges();
      }
    }

    private void ExecuteCloseCommand(Window window)
    {
      RemoveErrorsFromList();

      var unsavedItems = FilterManagerCollection.Where(p => p.CanUndo).ToList();

      if ( SaveButtonVisibility == Visibility.Visible && unsavedItems.Count > 0 )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerCloseUnsaveItem").ToString()) == MessageBoxResult.Yes )
        {
          ExecuteSaveCommandAsync().GetAwaiter().GetResult();
        }
        else
        {
          foreach ( var item in unsavedItems )
          {
            FilterManagerCollection.Remove(item);
          }
        }
      }

      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenFilterDataFromTailDataMessage>(OnOpenTailData);
      _cts.Cancel();
      window?.Close();
    }

    private void RemoveErrorsFromList()
    {
      var errors = GetFilterErrors();

      if ( errors.Count <= 0 )
        return;

      foreach ( var filterData in errors )
      {
        FilterManagerCollection.Remove(filterData);
      }

      OnPropertyChanged(nameof(FilterManagerView));
    }

    private List<FilterData> GetFilterErrors()
    {
      var errors = FilterManagerCollection.Where(p => p["Description"] != null || p["Filter"] != null || p["FilterSource"] != null || p["IsHighlight"] != null).ToList();
      return errors;
    }

    #endregion

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
