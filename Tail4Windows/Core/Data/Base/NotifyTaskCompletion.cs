using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Data.Base
{
  /// <inheritdoc />
  /// <summary>
  /// Notify task completion
  /// </summary>
  /// <typeparam name="TResult">Type of result</typeparam>
  public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
  {
    /// <summary>
    /// Task
    /// </summary>
    public Task<TResult> Task
    {
      get; private set;
    }

    /// <summary>
    /// Task result
    /// </summary>
    public TResult Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : default(TResult);

    /// <summary>
    /// Task status
    /// </summary>
    public TaskStatus Status
    {
      get => Task.Status;
    }

    /// <summary>
    /// Task is completed
    /// </summary>
    public bool IsCompleted => Task.IsCompleted;

    /// <summary>
    /// Task is not completed
    /// </summary>
    public bool IsNotCompleted => !Task.IsCompleted;

    /// <summary>
    /// Task is successfully completed
    /// </summary>
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

    /// <summary>
    /// Task canceled
    /// </summary>
    public bool IsCanceled => Task.IsCanceled;

    /// <summary>
    /// Task is faulted
    /// </summary>
    public bool IsFaulted => Task.IsFaulted;

    /// <summary>
    /// Task aggreate exception
    /// </summary>
    public AggregateException Exception => Task.Exception;

    /// <summary>
    /// Inner exception
    /// </summary>
    public Exception InnerException => Exception?.InnerException;

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage => InnerException?.Message;

    /// <summary>
    /// Task completion
    /// </summary>
    public Task TaskCompletion
    {
      get; private set;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="task">Task</param>
    public NotifyTaskCompletion(Task<TResult> task)
    {
      Task = task;

      if ( !task.IsCompleted )
      {
        TaskCompletion = WatchTaskAsync(task);
      }
    }

    private async Task WatchTaskAsync(Task task)
    {
      try
      {
        await task;

        // ReSharper disable once EmptyGeneralCatchClause
      }
      catch
      {

      }

      var propertyChanged = PropertyChanged;
      propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
      propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
      propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));

      if ( task.IsCanceled )
      {
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
      }
      else if ( task.IsFaulted )
      {
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InnerException)));
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
      }
      else
      {
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
      }
    }

    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
