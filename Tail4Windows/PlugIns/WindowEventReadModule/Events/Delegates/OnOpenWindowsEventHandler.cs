using Org.Vs.TailForWin.PlugIns.WindowEventReadModule.Events.Args;


namespace Org.Vs.TailForWin.PlugIns.WindowEventReadModule.Events.Delegates
{
  /// <summary>
  /// On open Windows event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="OnOpenWindowsEventArgs"/></param>
  public delegate void OnOpenWindowsEventHandler(object sender, OnOpenWindowsEventArgs e);
}
