using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.UI.Commands.Base
{
  /// <summary>
  /// Async command base
  /// </summary>
  public abstract class AsyncCommandBase : IAsyncCommand
  {
    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns></returns>
    public abstract bool CanExecute(object parameter);

    /// <summary>
    /// Execute async
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns></returns>
    public abstract Task ExecuteAsync(object parameter);

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    public async void Execute(object parameter)
    {
      await ExecuteAsync(parameter).ConfigureAwait(false);
    }

    /// <summary>
    /// Can execute changed event
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Raise can execute changed
    /// </summary>
    // ReSharper disable once MemberCanBeMadeStatic.Global
    protected void RaiseCanExecuteChanged()
    {
      CommandManager.InvalidateRequerySuggested();
    }
  }
}
