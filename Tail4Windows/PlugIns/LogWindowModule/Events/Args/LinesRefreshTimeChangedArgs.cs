using System;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args
{
  /// <summary>
  /// Lines refresh time changed event args
  /// </summary>
  public class LinesRefreshTimeChangedArgs : EventArgs
  {
    /// <summary>
    /// Lines read
    /// </summary>
    public int LinesRead
    {
      get;
    }

    /// <summary>
    /// Size refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
    }

    /// <summary>
    /// Standard constructur
    /// </summary>
    /// <param name="linesRead">Lines read</param>
    /// <param name="sizeRefreshTime">Size refresh time</param>
    public LinesRefreshTimeChangedArgs(int linesRead, string sizeRefreshTime)
    {
      LinesRead = linesRead;
      SizeRefreshTime = sizeRefreshTime;
    }
  }
}
