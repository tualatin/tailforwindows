using LiteDB;

namespace Org.Vs.Tail4Win.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// Session entity
  /// </summary>
  public class SessionEntity
  {
    /// <summary>
    /// ID
    /// </summary>
    [BsonId]
    public int SessionId
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
  }
}
