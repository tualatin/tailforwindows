namespace Org.Vs.Tail4Win.Business.StatisticEngine.Data.Messages
{
  /// <summary>
  /// Informs the statistic, to update current file
  /// </summary>
  public class StatisticUpdateReaderMessage
  {
    /// <summary>
    /// LogReader Id
    /// </summary>
    public Guid LogReaderId
    {
      get;
    }

    /// <summary>
    /// Index
    /// </summary>
    public int Index
    {
      get;
    }

    /// <summary>
    /// Name of file with path
    /// </summary>
    public string FileName
    {
      get;
    }

    /// <summary>
    /// Elapsed time as <see cref="TimeSpan"/>
    /// </summary>
    public TimeSpan ElapsedTime
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="fileName">Name of file with path</param>
    /// <param name="elapsedTime">Elapsed time as <see cref="TimeSpan"/></param>
    public StatisticUpdateReaderMessage(Guid logReaderId, int index, string fileName, TimeSpan elapsedTime)
    {
      LogReaderId = logReaderId;
      Index = index;
      FileName = fileName;
      ElapsedTime = elapsedTime;
    }
  }
}
