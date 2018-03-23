using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// EnvironmentOption view model
  /// </summary>
  public class EnvironmentOptionViewModel : NotifyMaster, IEnvironmentOptionViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(EnvironmentOptionViewModel));

    private CancellationTokenSource _cts;
    private readonly string _sendToLinkName;

    #region Properties

    private string _sendToButtonText;

    /// <summary>
    /// SendTo button text
    /// </summary>
    public string SendToButtonText
    {
      get => _sendToButtonText;
      set
      {
        if ( Equals(_sendToButtonText, value) )
          return;

        _sendToButtonText = value;
        OnPropertyChanged(nameof(SendToButtonText));
      }
    }

    #endregion


    /// <summary>
    /// Standard constructor
    /// </summary>
    public EnvironmentOptionViewModel()
    {
      _sendToLinkName = $"{Environment.GetFolderPath(Environment.SpecialFolder.SendTo)}\\{EnvironmentContainer.ApplicationTitle}.lnk";

      SetButtonContentByState();
    }

    #region Commands

    private IAsyncCommand _addToSendToCommand;

    /// <summary>
    /// AddToSendTo command
    /// </summary>
    public IAsyncCommand AddToSendToCommand => _addToSendToCommand ?? (_addToSendToCommand = AsyncCommand.Create((p, t) => ExecuteAddToSendToCommandAsync()));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteUnloadedCommand()));

    private ICommand _selectionChangedCommand;

    /// <summary>
    /// Selection changed command
    /// </summary>
    public ICommand SelectionChangedCommand => _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand(p => ExecuteSelectionChangedCommand()));

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new NotImplementedException();

    #endregion

    #region Command functions

    private void ExecuteSelectionChangedCommand()
    {
      switch ( SettingsHelperController.CurrentSettings.Language )
      {
      case EUiLanguage.English:

        LanguageSelector.SetLanguageResourceDictionary(EnvironmentContainer.ApplicationPath + @"\Language\en-EN.xaml");
        break;

      case EUiLanguage.German:

        LanguageSelector.SetLanguageResourceDictionary(EnvironmentContainer.ApplicationPath + @"\Language\de-DE.xaml");
        break;

      default:

        LanguageSelector.SetLanguageResourceDictionary(EnvironmentContainer.ApplicationPath + @"\Language\en-EN.xaml");
        break;
      }
    }

    private void ExecuteUnloadedCommand() => _cts?.Cancel();

    private async Task ExecuteAddToSendToCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

      await Task.Run(() =>
      {
        try
        {
          if ( File.Exists(_sendToLinkName) )
          {
            MouseService.SetBusyState();
            File.Delete(_sendToLinkName);
          }
          else
          {
            string message = string.Format(Application.Current.TryFindResource("QAddSendTo").ToString(), EnvironmentContainer.ApplicationTitle);

            if ( EnvironmentContainer.ShowQuestionMessageBox(message) == MessageBoxResult.No )
              return;

            MouseService.SetBusyState();
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortCut = shell.CreateShortcut(_sendToLinkName);
            shortCut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            shortCut.Save();
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          SetButtonContentByState();
        }
      }, _cts.Token).ConfigureAwait(false);
    }

    #endregion

    #region HelperFunctions

    private void SetButtonContentByState() => SendToButtonText = !File.Exists(_sendToLinkName) ? Application.Current.TryFindResource("EnvironmentSendTo").ToString() :
        Application.Current.TryFindResource("EnvironmentRemoveSentTo").ToString();

    #endregion
  }
}
