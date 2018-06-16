using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Open font settings message
  /// </summary>
  public class OpenFontSettingsMessage
  {
    /// <summary>
    /// Which window calls open font settings
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls open font settings</param>
    public OpenFontSettingsMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
