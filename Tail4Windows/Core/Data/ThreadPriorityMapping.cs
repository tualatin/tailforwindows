using System.Windows;


namespace Org.Vs.TailForWin.Core.Data
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
        try
        {
          var resourceKey = Application.Current.FindResource(ThreadPriority.ToString());

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