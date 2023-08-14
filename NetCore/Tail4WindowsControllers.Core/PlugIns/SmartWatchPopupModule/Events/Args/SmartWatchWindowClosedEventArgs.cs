namespace Org.Vs.TailForWin.Controllers.PlugIns.SmartWatchPopupModule.Events.Args
{
  /// <summary>
  /// SmartWatch window closed event args
  /// </summary>
  public class SmartWatchWindowClosedEventArgs : EventArgs
  {
    /// <summary>
    /// New tab window
    /// </summary>
    public bool NewTabWindow
    {
      get;
    }

    /// <summary>
    /// Filename
    /// </summary>
    public string FileName
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="newTab">New tab window</param>
    /// <param name="fileName">Filename</param>
    public SmartWatchWindowClosedEventArgs(bool newTab, string fileName)
    {
      NewTabWindow = newTab;
      FileName = fileName;
    }
  }
}
