using Org.Vs.Tail4Win.Business.Services.Events.Args;

namespace Org.Vs.Tail4Win.Business.Services.Events.Delegates
{
  /// <summary>
  /// Log entry created event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="LogEntryCreatedArgs"/></param>
  public delegate void LogEntryCreated(object sender, LogEntryCreatedArgs e);
}
