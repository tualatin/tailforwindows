using Org.Vs.TailForWin.Controllers.BaseView.Events.Args;


namespace Org.Vs.TailForWin.Controllers.BaseView.Events.Delegates
{
  /// <summary>
  /// File encoding changed event handler
  /// </summary>
  /// <param name="sender">Who sends the event</param>
  /// <param name="e"><see cref="FileEncodingArgs"/></param>
  public delegate void FileEncodingChangedEventHandler(object sender, FileEncodingArgs e);
}
