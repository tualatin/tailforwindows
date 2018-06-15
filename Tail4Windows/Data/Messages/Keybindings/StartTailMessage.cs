using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Start tail message
  /// </summary>
  public class StartTailMessage
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
    public StartTailMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
