using System;


namespace Org.Vs.TailForWin.Data.Messages.FindWhat
{
  /// <summary>
  /// FindWhat changed or closed message
  /// </summary>
  public class FindWhatChangedClosedMessage
  {
    /// <summary>
    /// Which window calls the FindWhat dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the FindWhat dialog</param>
    public FindWhatChangedClosedMessage(Guid windowGuid) => WindowGuid = windowGuid;
  }
}
