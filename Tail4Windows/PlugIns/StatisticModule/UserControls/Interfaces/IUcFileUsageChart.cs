using System.Collections.Generic;
using LiveCharts;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces
{
  /// <summary>
  /// UserControl file usage chart interface
  /// </summary>
  public interface IUcFileUsageChart : IChartUserControl
  {
    #region Properties

    /// <summary>
    /// PieChart series
    /// </summary>
    SeriesCollection PieSeries
    {
      get;
    }

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

    /// <summary>
    /// Average log count
    /// </summary>
    string AverageLogCount
    {
      get;
    }

    /// <summary>
    /// Average daily log count
    /// </summary>
    string AverageDailyLogCount
    {
      get;
    }

    /// <summary>
    /// Chart visibility
    /// </summary>
    List<ChartVisibility> ChartVisibility
    {
      get;
    }

    #endregion
  }
}
