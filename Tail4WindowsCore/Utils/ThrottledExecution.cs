using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using log4net;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Enable throttled execution from certain actions
  /// </summary>
  public sealed class ThrottledExecution : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ThrottledExecution));

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    private static readonly ConcurrentDictionary<string, ThrottledExecution> Throttles = new ConcurrentDictionary<string, ThrottledExecution>();
    private readonly string _scope;
    private Timer _timer;
    private Action _action;
    private Func<Task> _func;
    private int _throttleTimeInMs;
    private bool _invokeOnUiThread;
    private bool _beginInvokeOnUiThread;
    private Action<Exception> _exceptionHandler;
    private readonly object _timerLock = new object();


    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottledExecution" /> class.
    /// </summary>
    /// <param name="executionScope">Current execution scope</param>
    public ThrottledExecution([CallerMemberName] string executionScope = "")
    {
      _scope = executionScope;
      _timer = new Timer(OnTimerElapsedAsync, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Pause in milliseconds until execution will start
    /// </summary>
    /// <param name="msToThrottle">Milliseconds to start action</param>
    /// <returns><see cref="ThrottledExecution"/></returns>
    /// <exception cref="System.ArgumentException">If msToThrottle is less than zero</exception>
    public ThrottledExecution InMs(int msToThrottle)
    {
      if ( msToThrottle < 0 )
        Arg.NotNull(msToThrottle, nameof(msToThrottle));

      _throttleTimeInMs = msToThrottle;

      return this;
    }

    /// <summary>
    /// Action should use on UI thread
    /// </summary>
    /// <returns><see cref="ThrottledExecution"/></returns>
    /// <exception cref="Exception">If InvokeOnUiThread or BeginInvokeOnUiThread is in use</exception>
    public ThrottledExecution InvokeOnUiThread()
    {
      if ( _beginInvokeOnUiThread )
        throw new Exception("Either InvokeOnUiThread or BeginInvokeOnUiThread may be used");

      _invokeOnUiThread = true;

      return this;
    }

    /// <summary>
    /// Action should asynchron use on UI thread
    /// </summary>
    /// <returns><see cref="ThrottledExecution"/></returns>
    /// <exception cref="Exception">If InvokeOnUiThread or BeginInvokeOnUiThread is in use</exception>
    public ThrottledExecution BeginInvokeOnUiThread()
    {
      if ( _invokeOnUiThread )
        throw new Exception("Either InvokeOnUiThread or BeginInvokeOnUiThread may be used");

      _beginInvokeOnUiThread = true;

      return this;
    }

    /// <summary>
    /// What action should use
    /// </summary>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentNullException">If action is null</exception>
    public void Do(Action action)
    {
      Arg.NotNull(action, nameof(action));

      _action = action;

      DoInternal();
    }

    /// <summary>
    /// What asynchron action should use
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="exceptionHandler"></param>
    /// <exception cref="ArgumentNullException">If action is null</exception>
    /// <exception cref="Exception">If InokeOnUiThread or BeginInvokeOnUiThread not implemented for async action</exception>
    public void Do(Func<Task> action, Action<Exception> exceptionHandler)
    {
      Arg.NotNull(action, nameof(action));
      Arg.NotNull(exceptionHandler, nameof(exceptionHandler));

      if ( _invokeOnUiThread || _beginInvokeOnUiThread )
        throw new Exception("InvokeOnUiThread or BeginInvokeOnUiThread not implemented for DoAsync");

      _exceptionHandler = exceptionHandler;
      _func = action;

      DoInternal();
    }

    /// <summary>
    /// Sets the action, that should used, after throtteling
    /// </summary>
    /// <param name="continueWithAction">The action.</param>
    /// <exception cref="ArgumentNullException">If action is null</exception>
    public void ContinueWith(Action continueWithAction)
    {
      if ( continueWithAction == null )
        return;

      if ( _action == null && _func != null )
      {
        var action = _func;
        _func = async () =>
        {
          await action.Invoke().ConfigureAwait(false);
          continueWithAction.Invoke();
        };
      }
      else
      {
        if ( _action == null )
          return;

        var action = _action;
        _action = () =>
        {
          action.Invoke();
          continueWithAction.Invoke();
        };
      }
    }

    #region Private Methoden

    private async void OnTimerElapsedAsync(object state)
    {
      UnregisterMe();

      try
      {
        if ( _invokeOnUiThread )
        {
          _action.InvokeOnUiThread();
        }
        else if ( _beginInvokeOnUiThread )
        {
          _action.BeginInvokeOnUiThread();
        }
        else
        {
          if ( _action != null )
          {
            _action();
          }
          else
          {
            if ( _func != null )
            {

              try
              {
                await _func();
              }
              catch ( Exception ex )
              {
                if ( _exceptionHandler != null )
                  _exceptionHandler(ex);
                else
                  throw;
              }
            }
          }
        }
      }
      catch ( Exception ex )
      {
        Execute.BeginInvokeOnUiThread(() => throw ex);
      }

      Dispose();
    }

    private void UnregisterMe() => Throttles.TryRemove(_scope, out _);

    private void DoInternal()
    {
      if ( Throttles.TryGetValue(_scope, out var exe) )
        exe.Dispose();

      Throttles[_scope] = this;
      Reset();
    }

    private void Reset()
    {
      if ( Monitor.TryEnter(_timerLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          _timer?.Change(_throttleTimeInMs, Timeout.Infinite);
        }
        finally
        {
          Monitor.Exit(_timerLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    #endregion

    /// <summary>
    /// Releases all resources used by ThrottledExecution
    /// </summary>
    public void Dispose()
    {
      if ( Monitor.TryEnter(_timerLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          if ( _timer == null )
            return;

          _timer.Change(Timeout.Infinite, Timeout.Infinite);
          _timer.Dispose();
          _timer = null;
        }
        finally
        {
          Monitor.Exit(_timerLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }
  }
}
