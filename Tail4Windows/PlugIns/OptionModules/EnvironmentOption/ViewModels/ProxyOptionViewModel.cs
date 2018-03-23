using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Proxy option view model
  /// </summary>
  public class ProxyOptionViewModel : NotifyMaster, IProxyOptionViewModel
  {
    #region Commands

    private IAsyncCommand _passwordChangedCommand;

    /// <summary>
    /// PasswordChanged command
    /// </summary>
    public IAsyncCommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = AsyncCommand.Create((p, t) => ExecutePasswordChangedCommandAsync(p)));

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new System.NotImplementedException();

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new System.NotImplementedException();

    #endregion

    #region Command functions

    private async Task ExecutePasswordChangedCommandAsync(object parameter)
    {
      if ( !(parameter is RoutedEventArgs args) )
        return;

      if ( !(args.Source is PasswordBox passwordBox) )
        return;

      SettingsHelperController.CurrentSettings.ProxySettings.Password = await StringEncryption.EncryptAsync(passwordBox.Password).ConfigureAwait(false);
    }

    #endregion
  }
}
