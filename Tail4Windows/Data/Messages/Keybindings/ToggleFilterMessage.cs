using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Toggle filter message
  /// </summary>
  public class ToggleFilterMessage
  {
    /// <summary>
    /// Which window calls the Toggle filter
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the Toggle filter</param>
    public ToggleFilterMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
