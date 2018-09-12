using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Args;


namespace Org.Vs.TailForWin.Business.BookmarkEngine.Events.Delegates
{
  /// <summary>
  /// Bookmark data source changed event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="IdChangedEventArgs"/></param>
  public delegate void BookmarkDataSourceChangedEventHandler(object sender, IdChangedEventArgs e);
}
