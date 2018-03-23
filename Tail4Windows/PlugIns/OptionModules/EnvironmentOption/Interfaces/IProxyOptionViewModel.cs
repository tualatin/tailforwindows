using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces
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
