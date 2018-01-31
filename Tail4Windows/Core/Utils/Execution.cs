using System;
using System.Threading;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Extension method for execution on UI thread for an action
  /// </summary>
  public static class Execute
  {
    private static SynchronizationContext _uiSynchronizationContext;
    private static Thread _synchronizationContextThread;

    /// <summary>
    /// Check if current thread is the synchronization context
    /// </summary>
    /// <returns><c>True</c> if it is the synchronization context, otherwise <c>False</c></returns>
    public static bool CheckAccess() => Equals(Thread.CurrentThread, _synchronizationContextThread) || _synchronizationContextThread == null;

    /// <summary>
    /// Gets the current sychronization context
    /// </summary>
    public static SynchronizationContext UiSynchronizationContext
    {
      get => _uiSynchronizationContext;
      private set
      {
        if ( value != null )
        {
          value.Send(p => _synchronizationContextThread = Thread.CurrentThread, null);
          _uiSynchronizationContext = value;
        }
        else
        {
          _synchronizationContextThread = null;

        }
      }
    }

    /// <summary>
    /// Gets if an synchronization context is available
    /// </summary>
    public static bool IsUiSynchronizationContextAvailable => UiSynchronizationContext != null;

    /// <summary> 
    /// Initializes the class using the provided SynchronizationContext
    /// </summary> 
    public static void InitializeWithSynchronizationContext(SynchronizationContext synchronizationContext) => UiSynchronizationContext = synchronizationContext;

    /// <summary>
    /// Initializes the class using the current SynchronizationContext
    /// </summary>
    public static void Initialize() => InitializeWithSynchronizationContext(SynchronizationContext.Current);

    /// <summary> 
    /// Executes the action on the UI thread synchronously
    /// </summary> 
    /// <param name="action">The action to execute.</param> 
    /// <exception cref="InvalidOperationException">If UISynchronizationContext is null</exception>
    public static void InvokeOnUiThread(this Action action)
    {
      if ( UiSynchronizationContext == null )
        throw new InvalidOperationException("UISynchronizationContext not set.");

      UiSynchronizationContext.Send(p => action(), null);
    }

    /// <summary> 
    /// Executes the function on the UI thread synchronously
    /// </summary> 
    /// <param name="delegateMethod">The function to execute.</param> 
    /// <param name="args">Arguments</param> 
    /// <exception cref="InvalidOperationException">If UISynchronizationContext is null</exception>
    public static object InvokeOnUiThread(this Delegate delegateMethod, params object[] args)
    {
      if ( UiSynchronizationContext == null )
        throw new InvalidOperationException("UISynchronizationContext not set.");

      object result = null;
      UiSynchronizationContext.Send(p => result = delegateMethod.DynamicInvoke((object[]) p), args);

      return result;
    }

    /// <summary> 
    /// Executes the action on the UI thread asynchronously
    /// </summary> 
    /// <param name="action">The action to execute.</param> 
    /// <exception cref="InvalidOperationException">If UISynchronisationContext is null</exception>
    public static void BeginInvokeOnUiThread(this Action action)
    {
      if ( UiSynchronizationContext == null )
        throw new InvalidOperationException("UISynchronizationContext not set.");

      UiSynchronizationContext.Post(p => action(), null);
    }
  }
}
