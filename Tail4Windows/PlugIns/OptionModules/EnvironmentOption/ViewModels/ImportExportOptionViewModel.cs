using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Microsoft.Win32;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;
using Org.Vs.TailForWin.UI.UserControls;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Import/Export view model
  /// </summary>
  public class ImportExportOptionViewModel : NotifyMaster, IImportExportOptionViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ImportExportOptionViewModel));

    private CancellationTokenSource _cts;

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
      if ( EnvironmentContainer.ShowQuestionMessageBox(Application.Current.TryFindResource("QResetSettings").ToString()) == MessageBoxResult.No )
        return;

      MouseService.SetBusyState();

      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

      await EnvironmentContainer.Instance.ResetCurrentSettingsAsync(_cts).ConfigureAwait(false);
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ResetDataMessage(this));
    }

    private async Task ExecuteExportCommandAsync()
    {
      string appName = AppDomain.CurrentDomain.FriendlyName;
      string appSettings = $"{AppDomain.CurrentDomain.BaseDirectory}{appName}.Config";
      string date = DateTime.Now.ToString("yyyy_MM_dd_hh_mm");

      SaveFileDialog saveDialog = new SaveFileDialog
      {
        FileName = $"{date}_{appName}.Config",
        DefaultExt = ".export",
        Filter = Application.Current.TryFindResource("ImportExportExportSettingsFilter").ToString()
      };

      bool? result = saveDialog.ShowDialog();

      if ( result != true )
        return;

      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      MouseService.SetBusyState();

      try
      {
        FileStream saveFile = new FileStream(appSettings, FileMode.Open);
        Stream output = File.Create($"{saveDialog.FileName}.{saveDialog.DefaultExt}");
        byte[] buffer = new byte[1024];
        int len;

        while ( (len = await saveFile.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0 )
        {
          await output.WriteAsync(buffer, 0, len, _cts.Token).ConfigureAwait(false);
        }

        saveFile.Flush();
        output.Flush();

        saveFile.Close();
        output.Close();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private async Task ExecuteImportCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      if ( !EnvironmentContainer.OpenFileLogDialog(out string importSettings, Application.Current.TryFindResource("ImportExportExportSettingsFilter").ToString(),
        Application.Current.TryFindResource("OpenDialogImportSettings").ToString()) )
        return;

      if ( importSettings == null )
        return;

      if ( EnvironmentContainer.ShowQuestionMessageBox(Application.Current.TryFindResource("QImportSettings").ToString()) == MessageBoxResult.No )
        return;

      MouseService.SetBusyState();

      try
      {
        string appName = AppDomain.CurrentDomain.FriendlyName;
        FileStream importFile = new FileStream(importSettings, FileMode.Open);
        Stream output = File.Create($"{AppDomain.CurrentDomain.BaseDirectory}{appName}.Config");
        byte[] buffer = new byte[1024];
        int len;

        while ( (len = await importFile.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0 )
        {
          await output.WriteAsync(buffer, 0, len, _cts.Token).ConfigureAwait(false);
        }

        output.Flush();
        importFile.Flush();

        output.Close();
        importFile.Close();

        await EnvironmentContainer.Instance.ReloadSettingsAsync(_cts).ContinueWith(p => EnvironmentContainer.Instance.ReadSettingsAsync(_cts)).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    #endregion

    #region HelperFunctions

    private void ImportCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      CreatePopUpWindow(Application.Current.TryFindResource("ImportExportImportLabel").ToString(), Application.Current.TryFindResource("ImportExportImportFinishedMessage").ToString());
    }

    private void ResetCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      CreatePopUpWindow(Application.Current.TryFindResource("ImportExportResetConfiguration").ToString(), Application.Current.TryFindResource("ImportExportResetFinishedMessage").ToString());
    }

    private void ExportCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      CreatePopUpWindow(Application.Current.TryFindResource("ImportExportExportLabel").ToString(), Application.Current.TryFindResource("ImportExportExportFinishedMessage").ToString());
    }

    private void CreatePopUpWindow(string alert, string detail)
    {
      var alertPopUp = new FancyNotificationPopUp
      {
        PopUpAlert = alert,
        PopUpAlertDetail = detail
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowNotificationPopUpMessage(alertPopUp));
    }

    #endregion
  }
}
