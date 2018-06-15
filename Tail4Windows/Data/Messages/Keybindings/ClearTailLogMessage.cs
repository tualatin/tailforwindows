using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Clear tail log message
  /// </summary>
  public class ClearTailLogMessage
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
    public ClearTailLogMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
