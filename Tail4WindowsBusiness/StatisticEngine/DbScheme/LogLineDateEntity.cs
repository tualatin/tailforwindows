using System;


namespace Org.Vs.TailForWin.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// Log line date entity
  /// </summary>
  public class LogLineDateEntity
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
    /// <see cref="LogLineEntity"/>
    /// </summary>
    public LogLineEntity LogLine
    {
      get;
      set;
    }

    /// <summary>
    /// Date
    /// </summary>
    public DateTime Date
    {
      get;
      set;
    }
  }
}
