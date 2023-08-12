using System.Windows.Input;

namespace Org.Vs.Tail4Win.Controllers.Commands.Interfaces
{
  /// <inheritdoc />
  /// <summary>
  /// Async command interface
  /// </summary>
  public interface IAsyncCommand : ICommand
  {
    /// <summary>
    /// Execute async
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Task</returns>
    Task ExecuteAsync(object parameter);
  }
}
