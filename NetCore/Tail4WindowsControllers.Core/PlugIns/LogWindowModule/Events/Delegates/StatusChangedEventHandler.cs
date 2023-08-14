using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Args;

namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Delegates
{
  /// <summary>
  /// Status changed event handler
  /// </summary>
  /// <param name="sender">Whos is raising the event</param>
  /// <param name="args">Arguments of <see cref="StatusChangedArgs"/></param>
  public delegate void StatusChangedEventHandler(object sender, StatusChangedArgs args);
}
