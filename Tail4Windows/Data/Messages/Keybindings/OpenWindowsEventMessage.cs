using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Open Windows event message
  /// </summary>
  public class OpenWindowsEventMessage
  {
    /// <summary>
    /// Which window calls start tailing
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls start tailing</param>
    public OpenWindowsEventMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
