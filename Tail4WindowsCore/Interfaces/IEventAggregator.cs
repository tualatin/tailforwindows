using System;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Event aggreator interface
  /// </summary>
  public interface IEventAggregator
  {
    /// <summary>
    /// Send a message
    /// </summary>
    /// <typeparam name="T">Type of message</typeparam>
    /// <param name="message">Message to send</param>
    void SendMessage<T>(T message);

    /// <summary>
    /// Post a message
    /// </summary>
    /// <typeparam name="T">Type of message</typeparam>
    /// <param name="message">Message to send</param>
    void PostMessage<T>(T message);

    /// <summary>
    /// Register a message handler
    /// </summary>
    /// <typeparam name="T">Type of delegate</typeparam>
    /// <param name="eventHandler">Message handler to add</param>
    /// <returns>Registered delegate</returns>
    Action<T> RegisterHandler<T>(Action<T> eventHandler);

    /// <summary>
    /// Unregister a message handler
    /// </summary>
    /// <typeparam name="T">Type of delegate</typeparam>
    /// <param name="eventHandler">Message handler to remove</param>
    void UnregisterHandler<T>(Action<T> eventHandler);
  }
}
