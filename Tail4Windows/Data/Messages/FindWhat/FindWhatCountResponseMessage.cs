using System;


namespace Org.Vs.TailForWin.Data.Messages.FindWhat
{
  /// <summary>
  /// FindWhat count response message
  /// </summary>
  public class FindWhatCountResponseMessage
  {
    /// <summary>
    /// Count result
    /// </summary>
    public int Count
    {
      get;
    }

    /// <summary>
    /// Which window calls the find dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the find dialog</param>
    /// <param name="count">Count result</param>
    public FindWhatCountResponseMessage(Guid windowGuid, int count)
    {
      WindowGuid = windowGuid;
      Count = count;
    }
  }
}
