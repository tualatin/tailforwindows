using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Controllers;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Import/Export view model
  /// </summary>
  public class ImportExportOptionViewModel : NotifyMaster, IImportExportOptionViewModel
  {
    private CancellationTokenSource _cts;
    private readonly ISettingsDbController _dbSettingsController;
    private readonly IImportExportController _importExportController;

    #region Properties

    private string _currentSettingsPath;

    /// <summary>
    /// Current settings path
    /// </summary>
    public string CurrentSettingsPath
    {
      get => _currentSettingsPath;
      set
      {
        if ( Equals(value, _currentSettingsPath) )
          return;

        _currentSettingsPath = value;
        OnPropertyChanged(nameof(CurrentSettingsPath));
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ImportExportOptionViewModel()
    {
      _dbSettingsController = SettingsDbController.Instance;
      _importExportController = new ImportExportController();
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Import/Export loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create((p, t) => ExecuteImportLoadedCommandAsync()));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Import/Export unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteImportUnloadedCommand()));

    private IAsyncCommand _resetSettingsCommand;

    /// <summary>
    /// Reset current settings command
    /// </summary>
    public IAsyncCommand ResetSettingsCommand => _resetSettingsCommand ?? (_resetSettingsCommand = AsyncCommand.Create((p, t) => ExecuteResetSettingsCommandAsync()));

    private IAsyncCommand _exportCommand;

    /// <summary>
    /// Export current settings command
    /// </summary>
    public IAsyncCommand ExportCommand => _exportCommand ?? (_exportCommand = AsyncCommand.Create((p, t) => ExecuteExportCommandAsync()));

    private IAsyncCommand _importCommand;

    /// <summary>
    /// Import new settings command
    /// </summary>
    public IAsyncCommand ImportCommand => _importCommand ?? (_importCommand = AsyncCommand.Create((p, t) => ExecuteImportCommandAsync()));

    #endregion

    #region Command functions

    private async Task ExecuteImportLoadedCommandAsync()
    {
      await Task.Run(
        () =>
        {
          CurrentSettingsPath = $"{AppDomain.CurrentDomain.BaseDirectory}{AppDomain.CurrentDomain.FriendlyName}.Config";
          ((AsyncCommand<object>) ImportCommand).PropertyChanged += ImportCommandPropertyChanged;
          ((AsyncCommand<object>) ExportCommand).PropertyChanged += ExportCommandPropertyChanged;
          ((AsyncCommand<object>) ResetSettingsCommand).PropertyChanged += ResetCommandPropertyChanged;
        }).ConfigureAwait(false);
    }

    private void ExecuteImportUnloadedCommand()
    {
      ((AsyncCommand<object>) ImportCommand).PropertyChanged -= ImportCommandPropertyChanged;
      ((AsyncCommand<object>) ExportCommand).PropertyChanged -= ExportCommandPropertyChanged;
      ((AsyncCommand<object>) ResetSettingsCommand).PropertyChanged -= ResetCommandPropertyChanged;

      _cts?.Cancel();
    }

    private async Task ExecuteResetSettingsCommandAsync()
    {
      if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("QResetSettings").ToString()) == MessageBoxResult.No )
        return;

      MouseService.SetBusyState();

      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

      await EnvironmentContainer.Instance.ResetCurrentSettingsAsync(_cts).ConfigureAwait(false);
      await _dbSettingsController.ResetDbSettingsAsync(_cts.Token).ConfigureAwait(false);

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ResetDataMessage(this));
    }

    private async Task ExecuteExportCommandAsync()
    {
      string appName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
      string date = DateTime.Now.ToString("yyyy_MM_dd_hh_mm");
      string fileName = $"{date}_{appName}";

      if ( !InteractionService.OpenSaveDialog(ref fileName, Application.Current.TryFindResource("ImportExportExportSettingsFilter").ToString(),
        Application.Current.TryFindResource("ImportExportSaveDialogTitle").ToString()) )
      {
        return;
      }

      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      MouseService.SetBusyState();
      await _importExportController.ExportUserSettingsAsync(fileName, _cts.Token).ConfigureAwait(false);
    }

    private async Task ExecuteImportCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      if ( !InteractionService.OpenFileDialog(out string importSettings, Application.Current.TryFindResource("ImportExportExportSettingsFilter").ToString(),
        Application.Current.TryFindResource("OpenDialogImportSettings").ToString()) )
        return;

      if ( importSettings == null )
        return;

      if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("QImportSettings").ToString()) == MessageBoxResult.No )
        return;

      MouseService.SetBusyState();

      if ( await _importExportController.ImportUserSettingsAsync(importSettings, _cts.Token).ConfigureAwait(false) )
        await EnvironmentContainer.Instance.ReloadSettingsAsync(_cts).ContinueWith(p => EnvironmentContainer.Instance.ReadSettingsAsync(_cts)).ConfigureAwait(false);
    }

    #endregion

    #region HelperFunctions

    private void ImportCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      UiHelper.
        CreatePopUpWindow(Application.Current.TryFindResource("ImportExportImportLabel").ToString(), Application.Current.TryFindResource("ImportExportImportFinishedMessage").ToString());
    }

    private void ResetCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      UiHelper.
        CreatePopUpWindow(Application.Current.TryFindResource("ImportExportResetConfiguration").ToString(), Application.Current.TryFindResource("ImportExportResetFinishedMessage").ToString());
    }

    private void ExportCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      UiHelper.
        CreatePopUpWindow(Application.Current.TryFindResource("ImportExportExportLabel").ToString(), Application.Current.TryFindResource("ImportExportExportFinishedMessage").ToString());
    }

    #endregion
  }
}
