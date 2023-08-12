using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Proxy option view model interface
  /// </summary>
  public interface IProxyOptionViewModel : IOptionBaseViewModel
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
