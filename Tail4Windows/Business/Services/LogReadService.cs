using Org.Vs.TailForWin.Business.Interfaces;


namespace Org.Vs.TailForWin.Business.Services
{
  /// <summary>
  /// Log read service
  /// </summary>
  public class LogReadService : ILogReadService
  {
    /// <summary>
    /// Lines read
    /// </summary>
    public string LinesRead
    {
      get;
      private set;
    }

    /// <summary>
    /// Size and refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
      private set;
    }
  }
}
