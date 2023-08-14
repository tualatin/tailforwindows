using System.Windows;

namespace Org.Vs.TailForWin.Core.Data.Mappings
{
  /// <summary>
  /// System Threading priority enum mapping 
  /// </summary>
  public class ThreadPriorityMapping
  {
    /// <summary>
    /// Thread priority
    /// </summary>
    public ThreadPriority ThreadPriority
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
        try
        {
          var resourceKey = Application.Current.TryFindResource(ThreadPriority.ToString());

          return resourceKey?.ToString() ?? string.Empty;
        }
        catch
        {
          return string.Empty;
        }
      }
    }
  }
}
