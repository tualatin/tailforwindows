using log4net;
using Org.Vs.Tail4Win.Core.Logging;

namespace Org.Vs.Tail4Win.Core.Extensions
{
  /// <summary>
  /// <see cref="Task"/> utilities
  /// </summary>
  public static class TaskUtilities
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TaskUtilities));

    /// <summary>
    /// Runs a task save
    /// </summary>
    /// <param name="task"><see cref="Task"/> to be run</param>
    /// <param name="errorAction">Error handler</param>
    public static async void SafeAwait(this Task task, Action<Exception> errorAction = null)
    {
      try
      {
        await task.ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        errorAction?.Invoke(ex);
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }
  }
}
