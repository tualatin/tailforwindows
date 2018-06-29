using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Args;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Delegates
{
  /// <summary>
  /// Lines refresh time changed event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="LinesRefreshTimeChangedArgs"/></param>
  public delegate void LinesRefreshTimeChangedEventHandler(object sender, LinesRefreshTimeChangedArgs e);
}
