using System;
using System.Collections.Concurrent;
using System.Threading;


namespace Org.Vs.TailForWin.Core.Utils.Threading
{
  /// <summary>
  /// Service synchronisation context
  /// </summary>
  public class ServiceSynchronizationContext : SynchronizationContext
  {
    private readonly ConcurrentQueue<Tuple<SendOrPostCallback, object>> _queue;
    private readonly AutoResetEvent _signal;

    private bool _running = true;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public ServiceSynchronizationContext()
    {
      _signal = new AutoResetEvent(false);
      _queue = new ConcurrentQueue<Tuple<SendOrPostCallback, object>>();
      var t = new Thread(DoWork);

      t.Start();
    }

    private void DoWork()
    {
      while ( _running )
      {
        _signal.WaitOne(500);

        while ( _queue.TryDequeue(out var tuple) )
        {
          tuple.Item1(tuple.Item2);
        }
      }
    }

    /// <summary>
    /// Dispatches an asynchronous message to a synchronization context.
    /// </summary>
    /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
    /// <param name="state">The object passed to the delegate.</param>
    public override void Post(SendOrPostCallback d, object state)
    {
      _queue.Enqueue(new Tuple<SendOrPostCallback, object>(d, state));
      _signal.Set();
    }

    /// <summary>
    /// Dispatches a synchronous message to a synchronization context.
    /// </summary>
    /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
    /// <param name="state">The object passed to the delegate.</param>
    public override void Send(SendOrPostCallback d, object state)
    {
      var signal = new AutoResetEvent(false);
      var item = new Tuple<SendOrPostCallback, object>(
              p => {
                d(p);
                signal.Set();
              }, state);

      _queue.Enqueue(item);
      _signal.Set();

      signal.WaitOne();
    }

    /// <summary>
    /// Shutdown
    /// </summary>
    public void Shutdown()
    {
      _running = false;
      _signal.Set();
    }
  }
}
