using System;
using System.Collections.Generic;


namespace Org.Vs.TailForWin.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// Session entity
  /// </summary>
  public class SessionEntity
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SessionEntity() => Files = new List<FileEntity>();

    /// <summary>
    /// ID
    /// </summary>
    public int Id
    {
      get;
      set;
    }

    /// <summary>
    /// Date in <see cref="DateTime"/>
    /// </summary>
    public DateTime Date
    {
      get;
      set;
    }

    /// <summary>
    /// Session ID <see cref="Guid"/>
    /// </summary>
    public Guid Session
    {
      get;
      set;
    }

    /// <summary>
    /// Up time
    /// </summary>
    public TimeSpan UpTime
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

    /// <summary>
    /// <see cref="List{T}"/> of <see cref="FileEntity"/>
    /// </summary>
    public List<FileEntity> Files
    {
      get;
      set;
    }
  }
}
