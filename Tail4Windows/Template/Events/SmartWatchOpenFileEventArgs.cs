

namespace Org.Vs.TailForWin.Template.Events
{
  /// <summary>
  /// SmartWatchOpenFileEventArgs
  /// </summary>
  public class SmartWatchOpenFileEventArgs : EventArgs
  {
    private string fileFullPath;

    /// <summary>
    /// File with full path
    /// </summary>
    public string FileFullPath
    {
      get; 
      set
      {
        fileFullPath = value;
        FileWithoutPath = Path.GetFileName(value);
      }
    }

    /// <summary>
    /// File without path
    /// </summary>
    public string FileWithoutPath
    {
      get;
      private set;
    }

    /// <summary>
    /// Open in new tab
    /// </summary>
    public bool OpenInTab
    {
      get;
      set;
    }
  }
}
