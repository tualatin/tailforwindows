using System.Windows.Input;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// View model base interface
  /// </summary>
  public interface IViewModelBase
  {
    /// <summary>
    /// Loaded command
    /// </summary>
    IAsyncCommand LoadedCommand
    {
      get;
    }

    /// <summary>
    /// Unloaded command
    /// </summary>
    ICommand UnloadedCommand
    {
      get;
    }
  }
}
