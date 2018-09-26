namespace Org.Vs.TailForWin.Business.StatisticEngine.Data.Messages
{
  /// <summary>
  /// Shows the statistic, that file has changed
  /// </summary>
  public class StatisticChangeReaderMessage
  {
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
    /// Standard constructor
    /// </summary>
    /// <param name="index">Current index</param>
    /// <param name="fileName">Name of file with path</param>
    public StatisticChangeReaderMessage(int index, string fileName)
    {
      Index = index;
      FileName = fileName;
    }
  }
}
