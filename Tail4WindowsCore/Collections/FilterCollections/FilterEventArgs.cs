using System;


namespace Org.Vs.TailForWin.Core.Collections.FilterCollections
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
    /// Constructor
    /// </summary>
    /// <param name="isCompleted">Filtering is completed</param>
    /// <param name="exception">Occurred exception</param>
    public FilterEventArgs(bool isCompleted, Exception exception = null)
    {
      IsCompleted = isCompleted;
      Exception = exception;
    }
  }
}
