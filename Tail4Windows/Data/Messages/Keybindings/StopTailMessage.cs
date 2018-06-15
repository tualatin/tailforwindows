using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Stop tail message
  /// </summary>
  public class StopTailMessage
  {
    /// <summary>
    /// Which window calls stop tailing
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls stop tailing</param>
    public StopTailMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
