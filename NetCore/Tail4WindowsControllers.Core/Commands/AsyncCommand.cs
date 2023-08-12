using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using log4net;
using Org.Vs.Tail4Win.Controllers.Commands.Base;
using Org.Vs.Tail4Win.Core.Data.Base;
using Org.Vs.Tail4Win.Core.Logging;

namespace Org.Vs.Tail4Win.Controllers.Commands
{
  /// <summary>
  /// Async command
  /// </summary>
  /// <typeparam name="TResult">Type of result</typeparam>
  public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
  {
    private readonly Func<object, CancellationToken, Task<TResult>> _command;
    private readonly CancelAsyncCommand _cancelCommand;
    private NotifyTaskCompletion<TResult> _execution;
    private readonly Predicate<object> _canExecute;

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="command">Command</param>
    public AsyncCommand(Func<object, CancellationToken, Task<TResult>> command)
    {
      _command = command;
      _cancelCommand = new CancelAsyncCommand();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="canExecute">Can execute predicate</param>
    /// <param name="command">Command</param>
    public AsyncCommand(Predicate<object> canExecute, Func<object, CancellationToken, Task<TResult>> command)
    {
      _command = command;
      _cancelCommand = new CancelAsyncCommand();
      _canExecute = canExecute;
    }

    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns><c>True</c> if it can execute otherwise <c>false</c></returns>
    public override bool CanExecute(object parameter) => (Execution == null || Execution.IsCompleted) && (_canExecute == null || _canExecute(parameter));

    /// <summary>
    /// Execute async
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Task</returns>
    public override async Task ExecuteAsync(object parameter)
    {
      _cancelCommand.NotifyCommandStarting();
      Execution = NotifyTaskCompletion.Create(_command(parameter, _cancelCommand.Token));
      Execution.PropertyChanged += ExecutionPropertyChanged;

      RaiseCanExecuteChanged();

      await Execution.TaskCompleted;
      _cancelCommand.NotifyCommandFinished();

      RaiseCanExecuteChanged();
    }

    private void ExecutionPropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

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
      var handler = PropertyChanged;
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

        if ( !_cts.IsCancellationRequested )
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
    private static readonly ILog LOG = LogManager.GetLogger(typeof(AsyncCommand));

    /// <summary>
    /// Create async command
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<object> Create(Func<Task> command) =>
      new AsyncCommand<object>(async (param, _) =>
      {
        try
        {
          await command();
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        }
        return null;
      });

    /// <summary>
    /// Create async command
    /// </summary>
    /// <param name="canExecute">CanExecute</param>
    /// <param name="command">Comand</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<object> Create(Predicate<object> canExecute, Func<Task> command) =>
      new AsyncCommand<object>(canExecute, async (param, _) =>
      {
        try
        {
          await command();
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        }
        return null;
      });

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="canExecute">CanExecute</param>
    /// <param name="command">Command</param>
    /// <returns>AsyncCmmand of type object</returns>
    public static AsyncCommand<object> Create(Predicate<object> canExecute, Func<object, CancellationToken, Task> command) =>
      new AsyncCommand<object>(canExecute, async (param, token) =>
      {
        try
        {
          await command(param, token);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        }
        return null;
      });

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<object> Create(Func<object, Task> command) =>
      new AsyncCommand<object>(async (param, _) =>
      {
        try
        {
          await command(param);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        }
        return null;
      });

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command) => new AsyncCommand<TResult>((param, _) => command());

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<TResult> Create<TResult>(Func<object, Task<TResult>> command) => new AsyncCommand<TResult>((param, _) => command(param));

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="command">Command of cancellation token</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<object> Create(Func<CancellationToken, Task> command) =>
      new AsyncCommand<object>(async (param, token) =>
      {
        try
        {
          await command(token);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        }
        return null;
      });

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>AsyncCmmand of type object</returns>
    public static AsyncCommand<object> Create(Func<object, CancellationToken, Task> command) =>
      new AsyncCommand<object>(async (param, token) =>
      {
        try
        {
          await command(param, token);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        }
        return null;
      });

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command) => new AsyncCommand<TResult>(async (param, token) => await command(token));

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command</param>
    /// <returns>AsyncCommand of type object</returns>
    public static AsyncCommand<TResult> Create<TResult>(Func<object, CancellationToken, Task<TResult>> command) => new AsyncCommand<TResult>(command);
  }
}
