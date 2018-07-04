namespace Org.Vs.TailForWin.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// Log line entity
  /// </summary>
  public class LogLineEntity
  {
    /// <summary>
    /// Id
    /// </summary>
    public int Id
    {
      get;
      set;
    }

    /// <summary>
    /// Lines of log files
    /// </summary>
    public long Lines
    {
      get;
      set;
    }
  }
}
