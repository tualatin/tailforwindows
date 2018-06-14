using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Quick save message
  /// </summary>
  public class QuickSaveMessage
  {
    /// <summary>
    /// Which window calls the quick save
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the quick save</param>
    public QuickSaveMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
