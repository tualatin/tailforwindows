namespace Org.Vs.TailForWin.Business.SmartWatchEngine.Events.Delegates
{
  /// <summary>
  /// Smart watch files changes event handler
  /// </summary>
  /// <param name="sender">Who is firing the event</param>
  /// <param name="file">Full path of file</param>
  public delegate void SmartWatchFileChangedEventHandler(object sender, string file);
}
