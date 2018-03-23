using System.Windows.Input;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Environment option view model interface
  /// </summary>
  public interface IEnvironmentOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// AddToSendTo command
    /// </summary>
    IAsyncCommand AddToSendToCommand
    {
      get;
    }

    /// <summary>
    /// Selection changed command
    /// </summary>
    ICommand SelectionChangedCommand
    {
      get;
    }

    /// <summary>
    /// SendTo button text
    /// </summary>
    string SendToButtonText
    {
      get; set;
    }
  }
}
