using System.Windows;

namespace Org.Vs.Tail4Win.Core.Utils
{
  /// <summary>
  /// WeakEventListener
  /// </summary>
  public class WeakEventListener<TEventArgs> : IWeakEventListener where TEventArgs : EventArgs
  {
    private readonly EventHandler<TEventArgs> _handler;

    /// <summary>
    /// Initializes a new instance of WeakEventListener
    /// </summary>
    /// <param name="handler">The handler for the event</param>
    public WeakEventListener(EventHandler<TEventArgs> handler)
    {
      Arg.NotNull(handler, nameof(handler));
      _handler = handler;
    }

    /// <summary>
    /// Receives events from the centralized event manager
    /// </summary>
    /// <param name="managerType">The typ of the WeakEventManager calling this method</param>
    /// <param name="sender">Object that originated the event</param>
    /// <param name="e">Event data</param>
    /// <returns>
    /// <c>True</c> if the listener handled the event. It is considered an error by the WeakEventManager handling in WPF to register a listener for an event that the listener does not handle. 
    /// Regardless, the method should return false if it receives an event that it does not recognize or handle.
    /// </returns>
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      TEventArgs args = (TEventArgs) e;
      _handler(sender, args);

      return true;
    }
  }
}
