using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;

namespace Org.Vs.TailForWin.Business.StatisticEngine.Data.Extensions
{
  /// <summary>
  /// <see cref="IStatisticAnalysisCollection{T}"/> extension
  /// </summary>
  public static class StatisticAnalysisCollectionExtension
  {
    /// <summary>
    /// Get max file size
    /// </summary>
    /// <param name="collection"><see cref="IStatisticAnalysisCollection{T}"/> of <see cref="StatisticAnalysisData"/></param>
    /// <returns>Max file size if exists, otherwise 0</returns>
    public static double GetMaxFileSize(this IStatisticAnalysisCollection<StatisticAnalysisData> collection)
    {
      if ( collection == null || collection.Count == 0 )
        return 0;

      double maxFileSize;

      try
      {
        maxFileSize = collection.Select(p => p.Files.Where(f => !f.IsWindowsEvent)).Where(p => p.Any()).Select(p => p.Max(x => x.FileSizeTotalEvents)).Max(p => p);
      }
      catch
      {
        maxFileSize = 0;
      }
      return maxFileSize;
    }

    /// <summary>
    /// Get max Windows event size
    /// </summary>
    /// <param name="collection"><see cref="IStatisticAnalysisCollection{T}"/> of <see cref="StatisticAnalysisData"/></param>
    /// <returns>Max Windows event size, otherwise 0</returns>
    public static double GetMaxWindowsEventSize(this IStatisticAnalysisCollection<StatisticAnalysisData> collection)
    {
      if ( collection == null || collection.Count == 0 )
        return 0;

      double maxWindowsEvents;

      try
      {
        maxWindowsEvents = collection.Select(p => p.Files.Where(f => f.IsWindowsEvent)).Where(p => p.Any()).Select(p => p.Max(x => x.FileSizeTotalEvents)).Max(p => p);
      }
      catch
      {
        maxWindowsEvents = 0;
      }
      return maxWindowsEvents;
    }
  }
}
