using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Proxy option view model interface
  /// </summary>
  public interface IProxyOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// PasswordChanged command
    /// </summary>
    IAsyncCommand PasswordChangedCommand
    {
      get;
    }
  }
}
