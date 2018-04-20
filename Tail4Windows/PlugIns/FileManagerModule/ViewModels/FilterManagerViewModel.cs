using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels
{
  /// <summary>
  /// FilterManager view model
  /// </summary>
  public class FilterManagerViewModel : NotifyMaster
  {
    private CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;

    #region Properties

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get;
      set;
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
      get => FilterManagerView?.CurrentAddItem as FilterData;
      set
      {
        FilterManagerView.MoveCurrentTo(value);
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

      if ( CurrentTailData == null )
        return;

      if ( CurrentTailData.ListOfFilter.Count == 0 )
        CurrentTailData.ListOfFilter.Add(new FilterData());

      FilterManagerView = (ListCollectionView) new CollectionViewSource { Source = FilterManagerCollection }.View;
      FilterManagerCollectionViewHolder.Cv = FilterManagerView;
      SelectedItem = FilterManagerCollection.First();

      OnPropertyChanged(nameof(FilterManagerView));
      OnPropertyChanged(nameof(FilterManagerCollection));
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

    #endregion

    #region Command functions

    private bool CanExecuteUndo()
    {
      return true;
    }

    private void ExecuteUndoCommand()
    {

    }

    private bool CanExecuteSaveCommand()
    {
      if ( CurrentTailData == null )
        return false;

      var errors = FilterManagerCollection.Where(p => p["Description"] != null || p["Filter"] != null).ToList();
      bool undo = CanExecuteUndo();

      return errors.Count <= 0 && undo && CurrentTailData.IsLoadedByXml && FilterManagerCollection != null && FilterManagerCollection.Count > 0;
    }

    private void ExecuteAddFilterDataCommand()
    {
      var newItem = new FilterData();
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

      FilterManagerCollection.Remove(SelectedItem);
      await ExecuteSaveCommandAsync().ConfigureAwait(false);

      OnPropertyChanged(nameof(FilterManagerView));
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, CurrentTailData).ConfigureAwait(false);
    }

    private void ExecuteCloseCommand(Window window)
    {
      _cts.Cancel();
      window?.Close();
    }

    #endregion

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
