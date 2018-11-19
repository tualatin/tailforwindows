using System;
using System.Threading;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Async helper, to run async stuff as sync
  /// </summary>
  public static class AsyncHelper
  {
    private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    /// <summary>
    /// Runs a async task as sync
    /// </summary>
    /// <typeparam name="TResult">Return type</typeparam>
    /// <param name="func">Async function</param>
    /// <returns>Returns the result of async function</returns>
    public static TResult RunSync<TResult>(Func<Task<TResult>> func) => TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();

    /// <summary>
    /// Runs a async task as sync
    /// </summary>
    /// <param name="func">Async function</param>
    public static void RunSync(Func<Task> func) => TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
  }
}
