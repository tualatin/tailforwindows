using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Args;

namespace Org.Vs.TailForWin.Business.BookmarkEngine.Events.Delegates
{
  /// <summary>
  /// IdChanged event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="IdChangedEventArgs"/></param>
  public delegate void IdChangedEventHandler(object sender, IdChangedEventArgs e);
}
