﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule.Enums;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule.Interfaces;
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
    private readonly IFileManagerController _fileManagerController;
    private readonly IGlobalFilterController _globalFilterController;
    private bool _filterAdded;

    #region Properties

    private TailData _currentTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    private TailData CurrentTailData
    {
      get => _currentTailData;
      set
      {
        _currentTailData = value;
        SaveButtonVisibility = _currentTailData == null ? Visibility.Collapsed : (CurrentTailData.IsLoadedByXml ? Visibility.Visible : Visibility.Collapsed);

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
      _fileManagerController = new FileManagerController();
      _globalFilterController = new GlobalFilterController();

      ((AsyncCommand<object>) DeleteFilterDataCommand).PropertyChanged += OnDeletePropertyChanged;
      ((AsyncCommand<object>) LocalToGlobalFilterCommand).PropertyChanged += OnDeletePropertyChanged;

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

    private IAsyncCommand _localToGlobalFilterCommand;

    /// <summary>
    /// Converts a local filter to global
    /// </summary>
    public IAsyncCommand LocalToGlobalFilterCommand => _localToGlobalFilterCommand ?? (_localToGlobalFilterCommand = AsyncCommand.Create(p => SelectedItem != null, ExecuteLocalToGlobalFilterCommandAsync));

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
      if ( FilterManagerCollection == null || FilterManagerCollection.Count == 0 )
        return false;

      var unsavedItems = FilterManagerCollection.Where(p => p.CanUndo || p.FindSettingsData != null && p.FindSettingsData.CanUndo).ToList();

      return unsavedItems.Count > 0;
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

    private Task ExecuteDeleteCommandAsync() => DeleteFilterAsync();

    private bool CanExecuteSaveCommand()
    {
      if ( FilterManagerCollection == null || FilterManagerCollection.Count == 0 )
        return false;

      var errors = GetFilterErrors();
      bool undo = CanExecuteUndo();

      // Duplicate item?
      return !FilterManagerCollection
        .Where(p => !string.IsNullOrWhiteSpace(p.Filter))
        .GroupBy(p => p.Filter.ToLower())
        .Any(p => p.Count() > 1) &&
             (errors.Count <= 0 && undo && CurrentTailData.IsLoadedByXml && FilterManagerCollection != null && FilterManagerCollection.Count > 0);
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();
      await UpdateTailDataAsync();
    }

    private void ExecuteCloseCommand(Window window)
    {
      RemoveErrorsFromList();

      var unsavedItems = FilterManagerCollection.Where(p => p.CanUndo).ToList();

      // Duplicate item
      bool duplicates = FilterManagerCollection.GroupBy(p => p.Filter.ToLower()).Any(p => p.Count() > 1);

      if ( SaveButtonVisibility == Visibility.Visible && unsavedItems.Count > 0 && !duplicates )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerCloseUnsavedItem").ToString()) == MessageBoxResult.Yes )
        {
          // ReSharper disable once UnusedVariable
          var result = Task.Run(UpdateTailDataAsync).Result;
        }
        else
        {
          foreach ( FilterData item in unsavedItems )
          {
            while ( item.CanUndo )
            {
              item.Undo();
            }
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

      // 1. All undo
      foreach ( var filterData in errors )
      {
        while ( filterData.CanUndo )
        {
          filterData.Undo();
        }
      }

      // 2. Remove all errors from list
      errors = GetFilterErrors();

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
      if ( FilterManagerCollection == null )
        return new List<FilterData>();

      var errors = FilterManagerCollection.Where(p => p["Description"] != null || p["Filter"] != null).ToList();
      return errors;
    }

    private async Task ExecuteLocalToGlobalFilterCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      MouseService.SetBusyState();
      SetCancellationTokenSource();

      var globalFilters = await _globalFilterController.ReadGlobalFiltersAsync(_cts.Token).ConfigureAwait(false);
      var exists = globalFilters.SingleOrDefault(p => p.Id == SelectedItem.Id);

      if ( exists != null )
      {
        InteractionService.ShowInformationMessageBox(Application.Current.TryFindResource("FilterManagerLocalToGlobalAlreadyExists").ToString());
        return;
      }

      globalFilters.Add(SelectedItem);
      var success = await _globalFilterController.UpdateGlobalFilterAsync(globalFilters).ConfigureAwait(false);

      if ( !success )
      {
        InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("FilterManagerLocalToGlobalError").ToString());
        return;
      }

      EnvironmentContainer.Instance.CurrentEventManager.PostMessage(new StartStopTailMessage(EGlobalFilterState.Delete));
      await DeleteFilterAsync(false);
    }

    #endregion

    private async Task<bool> UpdateTailDataAsync()
    {
      SetCancellationTokenSource();

      await _fileManagerController.ReadJsonFileAsync(_cts.Token).ContinueWith(p =>
      {
        var success = _fileManagerController.UpdateTailDataAsync(CurrentTailData, _cts.Token, p.Result).Result;

        if ( !success )
        {
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("FileManagerSaveItemsError").ToString());
        }
      }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(false);

      foreach ( var item in FilterManagerCollection )
      {
        item.CommitChanges();
        item.FindSettingsData.CommitChanges();
      }

      return true;
    }

    private async Task DeleteFilterAsync(bool showMessageBox = true)
    {
      if ( SelectedItem == null )
        return;

      if ( !FilterManagerCollection.Contains(SelectedItem) )
        return;

      bool error = SelectedItem["Description"] != null || SelectedItem["Filter"] != null;

      if ( !error && showMessageBox )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerDeleteItemQuestion").ToString()) == MessageBoxResult.No )
          return;
      }

      MouseService.SetBusyState();

      if ( FilterManagerCollection.Contains(SelectedItem) )
        FilterManagerCollection.Remove(SelectedItem);

      if ( CurrentTailData.IsLoadedByXml || !showMessageBox )
        await ExecuteSaveCommandAsync().ConfigureAwait(false);
    }

    private void OnDeletePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      if ( SelectedItem == null )
        return;

      OnPropertyChanged(nameof(FilterManagerCollection));
      OnPropertyChanged(nameof(FilterManagerView));
    }


    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
