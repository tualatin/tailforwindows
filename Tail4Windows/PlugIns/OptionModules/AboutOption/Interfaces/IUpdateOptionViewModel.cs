using System.Windows.Input;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.Interfaces
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
