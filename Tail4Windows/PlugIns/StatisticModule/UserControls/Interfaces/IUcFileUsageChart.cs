namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces
{
  /// <summary>
  /// UserControl file usage chart interface
  /// </summary>
  public interface IUcFileUsageChart : IChartUserControl
  {
    /// <summary>
    /// Total lines read
    /// </summary>
    string TotalLinesRead
    {
      get;
    }
  }
}
