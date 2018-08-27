using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Show bookmark overview message
  /// </summary>
  public class ShowBookmarkOverviewMessage
  {
    /// <summary>
    /// Which window calls clear tail log
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls clear tail log</param>
    public ShowBookmarkOverviewMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
