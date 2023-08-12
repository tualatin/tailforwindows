using Org.Vs.Tail4Win.Controllers.PlugIns.LogWindowModule.Events.Args;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.LogWindowModule.Events.Delegates
{
  /// <summary>
  /// Selected lines changed event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="SelectedLinesChangedArgs"/></param>
  public delegate void SelectedLinesChangedEventHandler(object sender, SelectedLinesChangedArgs e);
}
