namespace Org.Vs.Tail4Win.Core.Collections.FilterCollections
{
  /// <summary>
  /// Filter event args
  /// </summary>
  public class FilterEventArgs : EventArgs
  {
    /// <summary>
    /// Filtering is completed
    /// </summary>
    public bool IsCompleted
    {
      get;
    }

    /// <summary>
    /// Occurred exception
    /// </summary>
    public Exception Exception
    {
      get;
    }

    /// <summary>
    /// Elapsed time
    /// </summary>
    public long ElapsedTime
    {
      get;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="isCompleted">Filtering is completed</param>
    /// <param name="elapsedTime">Elapsed time</param>
    /// <param name="exception">Occurred exception</param>
    public FilterEventArgs(bool isCompleted, long elapsedTime, Exception exception = null)
    {
      IsCompleted = isCompleted;
      ElapsedTime = elapsedTime;
      Exception = exception;
    }
  }
}
