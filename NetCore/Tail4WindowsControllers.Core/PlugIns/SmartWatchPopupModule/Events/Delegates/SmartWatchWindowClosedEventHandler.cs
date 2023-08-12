using Org.Vs.Tail4Win.Controllers.PlugIns.SmartWatchPopupModule.Events.Args;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.SmartWatchPopupModule.Events.Delegates
{
  /// <summary>
  /// SmartWatch window popup closed event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="SmartWatchWindowClosedEventArgs"/></param>
  public delegate void SmartWatchWindowClosedEventHandler(object sender, SmartWatchWindowClosedEventArgs e);
}
