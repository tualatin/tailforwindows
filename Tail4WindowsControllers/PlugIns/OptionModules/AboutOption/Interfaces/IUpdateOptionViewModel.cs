using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Interfaces
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
