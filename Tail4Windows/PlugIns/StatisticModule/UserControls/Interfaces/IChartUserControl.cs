using System;
using System.ComponentModel;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces
{
  /// <summary>
  /// Chart user control interface
  /// </summary>
  public interface IChartUserControl : INotifyPropertyChanged
  {
    /// <summary>
    /// Analysis collection
    /// </summary>
    IStatisticAnalysisCollection AnalysisCollection
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
  }
}
