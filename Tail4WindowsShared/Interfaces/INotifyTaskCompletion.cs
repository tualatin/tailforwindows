using System.ComponentModel;

namespace Org.Vs.Tail4Win.Shared.Interfaces
{
  /// <summary>
  /// Watches a task and raises property-changed notifications when the task completes.
  /// </summary>
  public interface INotifyTaskCompletion : INotifyPropertyChanged
  {
    /// <summary>
    /// Gets the task being watched.
    /// This property never changes and is never null.
    /// </summary>
    Task Task
    {
      get;
    }

    /// <summary>
    /// Gets the current task status.
    /// This property raises a notification when the task completes.
    /// </summary>
    TaskStatus Status
    {
      get;
    }

    /// <summary>
    /// Gets whether the task has completed.
    /// This property raises a notification when the value changes to true.
    /// </summary>
    bool IsCompleted
    {
      get;
    }

    /// <summary>
    /// Gets whether the task has completed successfully.
    /// This property raises a notification only if the task completes successfully (i.e., if the value changes to true).
    /// </summary>
    bool IsSuccessfullyCompleted
    {
      get;
    }

    /// <summary>
    /// Gets whether the task has been canceled.
    /// This property raises a notification only if the task is canceled (i.e., if the value changes to true).
    /// </summary>
    bool IsCanceled
    {
      get;
    }

    /// <summary>
    /// Gets whether the task has faulted.
    /// This property raises a notification only if the task faults (i.e., if the value changes to true).
    /// </summary>
    bool IsFaulted
    {
      get;
    }

    /// <summary>
    /// Gets the wrapped faulting exception for the task.
    /// Returns null if the task is not faulted.
    /// This property raises a notification only if the task faults (i.e., if the value changes to non-null).
    /// </summary>
    AggregateException Exception
    {
      get;
    }

    /// <summary>
    /// Gets the original faulting exception for the task.
    /// Returns null if the task is not faulted.
    /// This property raises a notification only if the task faults (i.e., if the value changes to non-null).
    /// </summary>
    Exception InnerException
    {
      get;
    }

    /// <summary>
    /// Gets the error message for the original faulting exception for the task.
    /// Returns null if the task is not faulted.
    /// This property raises a notification only if the task faults (i.e., if the value changes to non-null).
    /// </summary>
    string ErrorMessage
    {
      get;
    }
  }

  /// <summary>
  /// Watches a task and raises property-changed notifications when the task completes.
  /// </summary>
  /// <typeparam name="TResult">Type of result</typeparam>
  public interface INotifyTaskCompletion<TResult> : INotifyTaskCompletion
  {
    /// <summary>
    /// Gets the task being watched.
    /// This property never changes and is never <c>null</c>.
    /// </summary>
    new Task<TResult> Task
    {
      get;
    }

    /// <summary>
    /// Gets the result of the task.
    /// Returns the default value of <typeparamref name="TResult"/> if the task has not completed successfully.
    /// This property raises a notification when the task completes successfully.
    /// </summary>
    TResult Result
    {
      get;
    }
  }
}
