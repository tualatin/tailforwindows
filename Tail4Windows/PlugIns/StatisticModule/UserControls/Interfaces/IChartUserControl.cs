﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveCharts;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces
{
  /// <summary>
  /// Chart user control interface
  /// </summary>
  public interface IChartUserControl : INotifyPropertyChanged
  {
    /// <summary>
    /// Chart series
    /// </summary>
    SeriesCollection ChartSeries
    {
      get;
    }

    /// <summary>
    /// Reset current view settings (zoom, panning...)
    /// </summary>
    ICommand ResetViewCommand
    {
      get;
    }

    /// <summary>
    /// Updater tick command
    /// </summary>
    ICommand UpdaterTickCommand
    {
      get;
    }

    /// <summary>
    /// Can reset current view
    /// </summary>
    /// <returns>If command can execute <c>True</c> otherwise <c>False</c></returns>
    bool CanResetView();

    /// <summary>
    /// Analysis collection
    /// </summary>
    IStatisticAnalysisCollection<StatisticAnalysisData> AnalysisCollection
    {
      get;
      set;
    }

    /// <summary>
    /// XAxis formatter function
    /// </summary>
    Func<double, string> XAxisFormatter
    {
      get;
    }

    /// <summary>
    /// YAxis formatter function
    /// </summary>
    Func<double, string> YAxisFormatter
    {
      get;
    }

    /// <summary>
    /// Create a chart view async
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    Task CreateChartAsync();
  }
}
