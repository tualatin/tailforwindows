namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// System Threading priority enum mapping 
  /// </summary>
  public class ThreadPriorityMapping
  {
    /// <summary>
    /// Thread priority
    /// </summary>
    public System.Threading.ThreadPriority ThreadPriority
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description
    {
      get
      {
        switch(ThreadPriority)
        {
        case System.Threading.ThreadPriority.Lowest:

          return ("Lowest");

        case System.Threading.ThreadPriority.BelowNormal:

          return ("Below normal");

        case System.Threading.ThreadPriority.Normal:

          return ("Normal");

        case System.Threading.ThreadPriority.AboveNormal:

          return ("Above normal");

        case System.Threading.ThreadPriority.Highest:

          return ("Highest");

        default:

          return (string.Empty);
        }
      }
    }
  }
}