using System;


namespace Org.Vs.TailForWin.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// File entity
  /// </summary>
  public class FileEntity
  {
    /// <summary>
    /// ID
    /// </summary>
    public int Id
    {
      get;
      set;
    }

    /// <summary>
    /// Count of log output
    /// </summary>
    public long LogCount
    {
      get;
      set;
    }

    /// <summary>
    /// Time of running in <see cref="DateTime"/>
    /// </summary>
    public DateTime RunningTime
    {
      get;
      set;
    }

    /// <summary>
    /// Memory usage
    /// </summary>
    public long MemoryUsage
    {
      get;
      set;
    }
  }
}
