using Org.Vs.TailForWin.Business.Events.Args;


namespace Org.Vs.TailForWin.Business.Events.Delegates
{
  /// <summary>
  /// Log entry created event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="LogEntryCreatedArgs"/></param>
  public delegate void LogEntryCreated(object sender, LogEntryCreatedArgs e);
}
