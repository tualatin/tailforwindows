namespace Org.Vs.TailForWin.Template.Events
{
  /// <summary>
  /// Smart watch open file event handler
  /// </summary>
  /// <param name="sender">Who is firing the event</param>
  /// <param name="file">File name</param>
  /// <param name="openInTab">Open in new Tab window</param>
  public delegate void SmartWatchOpenFileEventHandler(object sender, string file, bool openInTab);
}
