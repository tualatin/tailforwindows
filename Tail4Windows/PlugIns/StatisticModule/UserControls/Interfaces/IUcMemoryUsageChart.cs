namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces
{
  /// <summary>
  /// UserControl memory usage chart interface
  /// </summary>
  public interface IUcMemoryUsageChart : IChartUserControl
  {
    /// <summary>
    /// Average running time
    /// </summary>
    string AverageRunningTime
    {
      get;
    }

    /// <summary>
    /// Minimum memory usage
    /// </summary>
    string MinMemoryUsage
    {
      get;
    }

    /// <summary>
    /// Average memory usage
    /// </summary>
    string AverageMemoryUsage
    {
      get;
    }

    /// <summary>
    /// Maximum memory usage
    /// </summary>
    string MaxMemoryUsage
    {
      get;
    }
  }
}
