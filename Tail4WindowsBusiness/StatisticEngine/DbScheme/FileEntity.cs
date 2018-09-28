using System;
using LiteDB;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;


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
    [BsonId]
    public int FileId
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
    /// Name of file with path
    /// </summary>
    public string FileName
    {
      get;
      set;
    }

    /// <summary>
    /// LogReader Id
    /// </summary>
    public Guid LogReaderId
    {
      get;
      set;
    }

    /// <summary>
    /// Is SmartWatch in use
    /// </summary>
    public bool IsSmartWatch
    {
      get;
      set;
    }

    /// <summary>
    /// Elapsed time as <see cref="TimeSpan"/>
    /// </summary>
    public TimeSpan? ElapsedTime
    {
      get;
      set;
    }

    /// <summary>
    /// Session reference
    /// </summary>
    [BsonRef(StatisticEnvironment.SessionEntityName)]
    public SessionEntity Session
    {
      get;
      set;
    }
  }
}
