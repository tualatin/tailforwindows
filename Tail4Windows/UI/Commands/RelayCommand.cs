using System;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI.Commands
{
  /// <inheritdoc />
  /// <summary>
  /// Relay command class
  /// </summary>
  public class RelayCommand : ICommand
  {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;


    /// <inheritdoc />
    /// <summary>
    /// Creates a new command that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    public RelayCommand(Action<object> execute)
      : this(null, execute)
    {

    }

    /// <summary>
    /// Creates a new command
    /// </summary>
    /// <param name="canExecute">Can execute</param>
    /// <param name="execute">Execute action</param>
    public RelayCommand(Predicate<object> canExecute, Action<object> execute)
    {
      _canExecute = canExecute;
      _execute = execute;
    }

    /// <summary>
    /// Can execute changed
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>True if it can execute otherwise false</returns>
    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

    /// <inheritdoc />
    /// <summary>
    /// Execute command
    /// </summary>
    /// <param name="parameter">Parameter</param>
    public void Execute(object parameter) => _execute(parameter);
  }
}
