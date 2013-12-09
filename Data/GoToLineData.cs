using System;


namespace TailForWin.Data
{
  /// <summary>
  /// Go to linenumber EventArgs Data class
  /// </summary>
  public class GoToLineData : EventArgs
  {
    /// <summary>
    /// LineNumber property
    /// </summary>
    public int LineNumber
    {
      get;
      set;
    }
  }
}
