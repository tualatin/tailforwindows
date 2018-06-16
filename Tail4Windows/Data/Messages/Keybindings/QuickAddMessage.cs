using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// Quick add message
  /// </summary>
  public class QuickAddMessage
  {
    /// <summary>
    /// Which window calls the quick add
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the quick add</param>
    public QuickAddMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
