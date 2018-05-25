using System;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Open GoToLine dialog message
  /// </summary>
  public class OpenGoToLineDialogMessage
  {
    /// <summary>
    /// Parent Guid
    /// </summary>
    public Guid ParentGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="parent"><see cref="Guid"/></param>
    public OpenGoToLineDialogMessage(Guid parent) => ParentGuid = parent;
  }
}
