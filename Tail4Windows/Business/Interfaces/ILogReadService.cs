namespace Org.Vs.TailForWin.Business.Interfaces
{
  /// <summary>
  /// Log read service interface
  /// </summary>
  public interface ILogReadService
  {
    /// <summary>
    /// Lines read
    /// </summary>
    string LinesRead
    {
      get;
    }

    /// <summary>
    /// Size refresh time
    /// </summary>
    string SizeRefreshTime
    {
      get;
    }
  }
}
