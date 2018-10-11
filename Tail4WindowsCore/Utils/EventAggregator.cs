using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Central event dispatcher used to send application messages to registered handlers
  /// </summary>
  public class EventAggregator : IEventAggregator
  {
    /// <summary>
    /// Storage for all our registered handlers
    /// </summary>
    private readonly List<Delegate> _handlers = new List<Delegate>();

    /// <summary>
    /// SynchronizationContext used to transition to the correct thread
    /// </summary>
    private readonly SynchronizationContext _synchronizationContext;

    /// <summary>
    /// Initializes a new instance of the EventAggregator class.
    /// </summary>
    public EventAggregator() => _synchronizationContext = SynchronizationContext.Current;

    /// <summary>
    /// Send a message instance for immediate delivery
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    /// <param name="message">Message to send</param>
    public void SendMessage<T>(T message)
    {
      if ( message == null )
        return;

      if ( _synchronizationContext != null )
        _synchronizationContext.Send(m => Dispatch((T) m), message);
      else
        Dispatch(message);
    }

    /// <summary>
    /// Post a message instance for asynchronous delivery
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    /// <param name="message">Message to send</param>
    public void PostMessage<T>(T message)
    {
      if ( message == null )
        return;

      if ( _synchronizationContext != null )
        _synchronizationContext.Post(m => Dispatch((T) m), message);
      else
        Dispatch(message);
    }

    /// <summary>
    /// Register a message handler
    /// </summary>
    /// <param name="eventHandler">Message handler to add.</param>
    public Action<T> RegisterHandler<T>(Action<T> eventHandler)
    {
      Arg.NotNull(eventHandler, nameof(eventHandler));

      _handlers.Add(eventHandler);
      return eventHandler;
    }

    /// <summary>
    /// Unregister a message handler
    /// </summary>
    /// <param name="eventHandler">Message handler to remove.</param>
    public void UnregisterHandler<T>(Action<T> eventHandler)
    {
      Arg.NotNull(eventHandler, nameof(eventHandler));
      _handlers.Remove(eventHandler);
    }

    /// <summary>
    /// Dispatch a message to all appropriate handlers
    /// </summary>
    /// <typeparam name="T">Type of the message</typeparam>
    /// <param name="message">Message to dispatch to registered handlers</param>
    private void Dispatch<T>(T message)
    {
      Arg.NotNull(message, nameof(message));

      var compatibleHandlers = new List<Action<T>>();

      try
      {
        compatibleHandlers = _handlers.OfType<Action<T>>().ToList();
      }
      catch
      {
        Type msgType = typeof(Action<T>);

        for ( int i = _handlers.Count - 1; i >= 0; i-- )
        {
          Type type = _handlers[i].GetType();

          if ( msgType != type )
            continue;

          var action = _handlers[i] as Action<T>;
          compatibleHandlers.Add(action);
        }
      }

      foreach ( var h in compatibleHandlers )
      {
        h(message);
      }
    }
  }
}
