using System;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Open TailManager message
  /// </summary>
  public class OpenTailManagerMessage
  {
    /// <summary>
    /// Which window calls the TailManager dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the TailManager dialog</param>
    public OpenTailManagerMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
