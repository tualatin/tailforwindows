using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.Commands.Base;


namespace Org.Vs.TailForWin.UI.Commands
{
  /// <summary>
  /// Async command
  /// </summary>
  /// <typeparam name="TResult">Type of result</typeparam>
  // ReSharper disable once InheritdocConsiderUsage
  public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
  {
    private readonly Func<CancellationToken, Task<TResult>> _command;
    private readonly CancelAsyncCommand _cancelCommand;
    private NotifyTaskCompletion<TResult> _execution;

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="command">Command</param>
    public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
    {
      _command = command;
      _cancelCommand = new CancelAsyncCommand();
    }

    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Paremeter</param>
    /// <returns><c>True</c> if it can execute otherwise <c>false</c></returns>
    public override bool CanExecute(object parameter) => Execution == null || Execution.IsCompleted;

    /// <summary>
    /// Execute async
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Task</returns>
    public override async Task ExecuteAsync(object parameter)
    {
      _cancelCommand.NotifyCommandStarting();
      Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token));

      RaiseCanExecuteChanged();

      await Execution.TaskCompletion;
      _cancelCommand.NotifyCommandFinished();

      RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Cancel command
    /// </summary>
    public ICommand CancelCommand => _cancelCommand;

    /// <summary>
    /// Execution
    /// </summary>
    public NotifyTaskCompletion<TResult> Execution
    {
      get => _execution;
      private set
      {
        _execution = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// PropertyChanged event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc />
    /// <summary>
    /// Cancel async command
    /// </summary>
    private sealed class CancelAsyncCommand : ICommand
    {
      private CancellationTokenSource _cts = new CancellationTokenSource();
      private bool _commandExecuting;

      /// <summary>
      /// Cancel token
      /// </summary>
      public CancellationToken Token => _cts.Token;

      /// <summary>
      /// Notify command starting
      /// </summary>
      public void NotifyCommandStarting()
      {
        _commandExecuting = true;

        if (!_cts.IsCancellationRequested)
          return;

        _cts = new CancellationTokenSource();
        RaiseCanExecuteChanged();
      }

      /// <summary>
      /// Notify command finished
      /// </summary>
      public void NotifyCommandFinished()
      {
        _commandExecuting = false;
        RaiseCanExecuteChanged();
      }

      bool ICommand.CanExecute(object parameter) => _commandExecuting && !_cts.IsCancellationRequested;

      void ICommand.Execute(object parameter)
      {
        _cts.Cancel();
        RaiseCanExecuteChanged();
      }

      /// <summary>
      /// Can execute changed event
      /// </summary>
      public event EventHandler CanExecuteChanged
      {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
      }

      private void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
  }

  /// <summary>
  /// Async command
  /// </summary>
  public static class AsyncCommand
  {
    /// <summary>
    /// Create
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<object> Create(Func<Task> command)
    {
      return new AsyncCommand<object>(async _ => { await command(); return null; });
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command) => new AsyncCommand<TResult>(_ => command());

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="command">Command of cancellation token</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<object> Create(Func<CancellationToken, Task> command)
    {
      return new AsyncCommand<object>(async token => { await command(token); return null; });
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command) => new AsyncCommand<TResult>(command);
  }
}
