using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// GlobalHighlightOption view model
  /// </summary>
  public class GlobalHighlightOptionViewModel : NotifyMaster, IGlobalHighlightOptionViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(GlobalHighlightOptionViewModel));

    private readonly IGlobalFilterController _filterController;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public GlobalHighlightOptionViewModel()
    {
      _filterController = new GlobalFilterController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnSavePropertyChanged;
      ((AsyncCommand<object>) SaveCommand).PropertyChanged += OnSavePropertyChanged;
      ((AsyncCommand<object>) DeleteHighlightColorCommand).PropertyChanged += OnDeletePropertyChanged;
    }

    /// <summary>
    /// Current selected item
    /// </summary>
    public FilterData SelectedItem
    {
      get => FilterManagerView?.CurrentItem as FilterData;
      set
      {
        FilterManagerView?.MoveCurrentTo(value);
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

    /// <summary>
    /// Global highlight collection changed
    /// </summary>
    public bool GlobalHighlightCollectionChanged
    {
      get;
      private set;
    }

    private ObservableCollection<FilterData> _globalHighlightCollection;

    /// <summary>
    /// Global highlight collection
    /// </summary>
    public ObservableCollection<FilterData> GlobalHighlightCollection
    {
      get => _globalHighlightCollection;
      private set
      {
        if ( value == _globalHighlightCollection )
          return;

        _globalHighlightCollection = value;
        OnPropertyChanged();
      }
    }

    private IAsyncCommand _saveCommand;

    /// <summary>
    /// Saves current collection
    /// </summary>
    public IAsyncCommand SaveCommand => _saveCommand ?? (_saveCommand = AsyncCommand.Create(p => CanExecuteSaveCommand(), ExecuteSaveCommandAsync));

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteUnloadedCommand()));

    private ICommand _addHighlightColorCommand;

    /// <summary>
    /// Add highlight color to source
    /// </summary>
    public ICommand AddHighlightColorCommand => _addHighlightColorCommand ?? (_addHighlightColorCommand = new RelayCommand(p => ExecuteAddHighlightColorCommand()));

    private IAsyncCommand _deleteHighlightColorCommand;

    /// <summary>
    /// Delete highlight color from source
    /// </summary>
    public IAsyncCommand DeleteHighlightColorCommand => _deleteHighlightColorCommand ?? (_deleteHighlightColorCommand = AsyncCommand.Create(p => CanExecuteDeleteHighlightColorCommand(), ExecuteDeleteHighlightColorCommandAsync));

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndo(), p => ExecuteUndoCommand()));

    private async Task ExecuteLoadedCommandAsync()
    {
      SetCancellationTokenSource();

      _globalHighlightCollection = await _filterController.ReadGlobalFiltersAsync(_cts.Token).ConfigureAwait(false);
      _globalHighlightCollection.CollectionChanged += OnGlobalHighlightCollectionChanged;

      CommitChanges();
    }

    private void ExecuteUnloadedCommand()
    {
      _cts.Cancel();
      GlobalHighlightCollection.CollectionChanged -= OnGlobalHighlightCollectionChanged;
      GlobalHighlightCollection.Clear();
    }

    private void ExecuteAddHighlightColorCommand()
    {
      var newItem = new FilterData { IsGlobal = true };
      newItem.CommitChanges();
      newItem.FindSettingsData.CommitChanges();

      GlobalHighlightCollection.Add(newItem);
      SelectedItem = GlobalHighlightCollection.Last();

      // Whaaaaaaat? 
      try
      {
        OnPropertyChanged(nameof(FilterManagerView));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private bool CanExecuteDeleteHighlightColorCommand() => SelectedItem != null && GlobalHighlightCollection.Contains(SelectedItem);

    private async Task ExecuteDeleteHighlightColorCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      if ( !GlobalHighlightCollection.Contains(SelectedItem) )
        return;

      bool error = SelectedItem["Description"] != null || SelectedItem["Filter"] != null;

      if ( !error )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerDeleteItemQuestion").ToString()) == MessageBoxResult.No )
          return;
      }

      MouseService.SetBusyState();

      var success = await _filterController.DeleteGlobalFilterAsync(SelectedItem.Id).ConfigureAwait(false);

      if ( !success )
        InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("FileManagerDeleteTailDataItemError").ToString());
    }

    private bool CanExecuteUndo()
    {
      if ( GlobalHighlightCollection == null || GlobalHighlightCollection.Count == 0 )
        return false;

      var unsavedItems = GlobalHighlightCollection.Where(p => p.CanUndo || p.FindSettingsData != null && p.FindSettingsData.CanUndo).ToList();

      return unsavedItems.Count > 0;
    }

    private void ExecuteUndoCommand()
    {
      SelectedItem?.Undo();
      SelectedItem?.FindSettingsData?.Undo();
    }

    private bool CanExecuteSaveCommand()
    {
      if ( GlobalHighlightCollection == null || GlobalHighlightCollection.Count == 0 )
        return false;

      var errors = GetFilterErrors();
      bool undo = CanExecuteUndo();

      // Duplicate item?
      return !GlobalHighlightCollection.Where(p => !string.IsNullOrWhiteSpace(p.Filter))
        .GroupBy(p => p.Filter.ToLower())
        .Any(p => p.Count() > 1) &&
             (errors.Count <= 0 &&
              undo &&
              GlobalHighlightCollection != null &&
              GlobalHighlightCollection.Count > 0);
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

      var collection = new ObservableCollection<FilterData>(GlobalHighlightCollection);
      var success = await _filterController.UpdateGlobalFilterAsync(collection).ConfigureAwait(false);

      if ( !success )
      {
        InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("FileManagerSaveItemsError").ToString());
      }

      CommitChanges();
    }

    private void CommitChanges()
    {
      foreach ( var item in _globalHighlightCollection )
      {
        item.CommitChanges();
        item.FindSettingsData.CommitChanges();
      }
    }

    private void OnSavePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      if ( _globalHighlightCollection == null )
      {
        _globalHighlightCollection = new ObservableCollection<FilterData>();

        CommitChanges();
      }

      FilterManagerView = (ListCollectionView) new CollectionViewSource { Source = GlobalHighlightCollection }.View;
      FilterManagerCollectionViewHolder.Cv = FilterManagerView;

      if ( FilterManagerView.Count == 0 )
        return;

      SelectedItem = GlobalHighlightCollection.Last();
      GlobalHighlightCollectionChanged = true;

      OnPropertyChanged(nameof(GlobalHighlightCollection));
      OnPropertyChanged(nameof(FilterManagerView));
    }

    private void OnDeletePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      if ( SelectedItem == null )
        return;

      if ( GlobalHighlightCollection.Contains(SelectedItem) )
        GlobalHighlightCollection.Remove(SelectedItem);

      OnPropertyChanged(nameof(GlobalHighlightCollection));
      OnPropertyChanged(nameof(FilterManagerView));
    }

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }

    private List<FilterData> GetFilterErrors()
    {
      if ( GlobalHighlightCollection == null )
        return new List<FilterData>();

      var errors = GlobalHighlightCollection.Where(p => p["Description"] != null || p["Filter"] != null).ToList();
      return errors;
    }

    private void OnGlobalHighlightCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => GlobalHighlightCollectionChanged = true;
  }
}
