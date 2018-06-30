using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AlertOption.Interfaces
{
  /// <summary>
  /// Alert option view model interface
  /// </summary>
  public interface IAlertOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// Preview drag enter command
    /// </summary>
    ICommand PreviewDragEnterCommand
    {
      get;
    }

    /// <summary>
    /// Select sound file command
    /// </summary>
    ICommand SelectSoundFileCommand
    {
      get;
    }

    /// <summary>
    /// Send test mail command
    /// </summary>
    IAsyncCommand SendTestMailCommand
    {
      get;
    }
  }
}
