using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Show extended toolbar message
  /// </summary>
  public class ShowExtendedToolbarMessage
  {
    /// <summary>
    /// Which window calls the show extended toolbar
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the show extended toolbar</param>
    public ShowExtendedToolbarMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
