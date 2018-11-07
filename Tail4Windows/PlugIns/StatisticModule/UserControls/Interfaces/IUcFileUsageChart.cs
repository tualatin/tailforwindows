namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces
{
  /// <summary>
  /// UserControl file usage chart interface
  /// </summary>
  public interface IUcFileUsageChart : IChartUserControl
  {
    #region Properties

    /// <summary>
    /// Total lines read
    /// </summary>
    string TotalLinesRead
    {
      get;
    }

    /// <summary>
    /// Average daily file count
    /// </summary>
    string AverageDailyFileCount
    {
      get;
    }

    #endregion
  }
}
