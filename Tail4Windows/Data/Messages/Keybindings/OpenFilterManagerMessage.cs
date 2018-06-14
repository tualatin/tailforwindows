using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Open FilterManager message
  /// </summary>
  public class OpenFilterManagerMessage
  {
    /// <summary>
    /// Which window calls the FilterManager dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the FilterManager dialog</param>
    public OpenFilterManagerMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
