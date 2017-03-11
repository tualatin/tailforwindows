using System;


namespace Org.Vs.TailForWin.Data.Events
{
  /// <summary>
  /// WrapAroundBool object
  /// </summary>
  public class WrapAroundBool : EventArgs
  {
    /// <summary>
    /// Wrap boolean
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }
  }
}
