using System.Windows.Input;
using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;
using Org.Vs.Tail4Win.Controllers.UI.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.AboutOption.Interfaces
{
  /// <summary>
  /// Update option view model interface
  /// </summary>
  public interface IUpdateOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// Check update command
    /// </summary>
    IAsyncCommand CheckUpdateCommand
    {
      get;
    }

    /// <summary>
    /// Visivit website command
    /// </summary>
    ICommand VisitWebsiteCommand
    {
      get;
    }
  }
}
