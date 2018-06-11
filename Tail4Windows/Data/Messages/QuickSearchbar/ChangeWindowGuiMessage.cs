using System;


namespace Org.Vs.TailForWin.Data.Messages.QuickSearchbar
{
  /// <summary>
  /// Change window <see cref="Guid"/> message
  /// </summary>
  public class ChangeWindowGuiMessage
  {
    /// <summary>
    /// Current window <see cref="Guid"/>
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Current window <see cref="Guid"/></param>
    public ChangeWindowGuiMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
