using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// GlobalHighlightOption view model
  /// </summary>
  public class GlobalHighlightOptionViewModel : NotifyMaster, IGlobalHighlightOptionViewModel
  {
    private readonly IGlobalFilterController _filterController;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public GlobalHighlightOptionViewModel()
    {
      _filterController = new GlobalFilterController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnSavePropertyChanged;
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

    private async Task ExecuteLoadedCommandAsync()
    {
      SetCancellationTokenSource();
      _globalHighlightCollection = await _filterController.ReadGlobalFiltersAsync(_cts.Token).ConfigureAwait(false);
    }

    private void ExecuteUnloadedCommand()
    {
      _cts.Cancel();
      GlobalHighlightCollection.Clear();
    }

    private void OnSavePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      OnPropertyChanged(nameof(GlobalHighlightCollection));
    }

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
