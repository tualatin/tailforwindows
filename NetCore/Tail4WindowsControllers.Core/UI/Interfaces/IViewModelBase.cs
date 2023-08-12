using System.Windows.Input;
using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.UI.Interfaces
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
