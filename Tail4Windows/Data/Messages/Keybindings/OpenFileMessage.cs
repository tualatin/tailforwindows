using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// OpenFile message
  /// </summary>
  public class OpenFileMessage
  {
    /// <summary>
    /// Which window calls the OpenFile dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the OpenFile dialog</param>
    public OpenFileMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
