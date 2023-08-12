using Org.Vs.Tail4Win.Controllers.PlugIns.WindowsEventReadModule.Events.Args;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.WindowsEventReadModule.Events.Delegates
{
  /// <summary>
  /// On open Windows event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="OnOpenWindowsEventArgs"/></param>
  public delegate void OnOpenWindowsEventHandler(object sender, OnOpenWindowsEventArgs e);
}
