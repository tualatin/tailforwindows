using System.Windows;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Args
{
  /// <summary>
  /// Lines refresh time changed event args
  /// </summary>
  public class LinesRefreshTimeChangedArgs : RoutedEventArgs
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
    /// Standard constructor
    /// </summary>
    /// <param name="routedEvent"><see cref="RoutedEvent"/></param>
    /// <param name="linesRead">Lines read</param>
    /// <param name="sizeRefreshTime">Size refresh time</param>
    public LinesRefreshTimeChangedArgs(RoutedEvent routedEvent, int linesRead, string sizeRefreshTime)
      : base(routedEvent)
    {
      LinesRead = linesRead;
      SizeRefreshTime = sizeRefreshTime;
    }
  }
}
