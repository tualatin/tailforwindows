namespace Org.Vs.Tail4Win.Business.StatisticEngine.Data.Messages
{
  /// <summary>
  /// Informs the statistic, that file has changed
  /// </summary>
  public class StatisticChangeReaderMessage
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
    /// Is Windows event
    /// </summary>
    public bool IsWindowsEvent
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="fileName">Name of file with path</param>
    /// <param name="isWindowsEvent">Is Windows event</param>
    public StatisticChangeReaderMessage(Guid logReaderId, int index, string fileName, bool isWindowsEvent)
    {
      LogReaderId = logReaderId;
      Index = index;
      FileName = fileName;
      IsWindowsEvent = isWindowsEvent;
    }
  }
}
