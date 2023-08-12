using System.Windows.Input;
using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;
using Org.Vs.Tail4Win.Controllers.UI.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.AlertOption.Interfaces
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
