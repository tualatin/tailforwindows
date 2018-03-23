using System.Windows.Input;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption.Interfaces
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
