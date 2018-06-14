using System;


namespace Org.Vs.TailForWin.Data.Messages.Keybindings
{
  /// <summary>
  /// GoToLine message
  /// </summary>
  public class GoToLineMessage
  {
    /// <summary>
    /// Go to index
    /// </summary>
    public int Index
    {
      get;
    }

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
    /// <param name="index">Index</param>
    /// <param name="parent"><see cref="Guid"/></param>
    public GoToLineMessage(int index, Guid parent)
    {
      ParentGuid = parent;
      Index = index;
    }
  }
}
