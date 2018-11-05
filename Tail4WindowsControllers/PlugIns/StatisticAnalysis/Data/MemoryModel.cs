using System;
using LiveCharts.Defaults;


namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data
{
  /// <summary>
  /// Memory model
  /// </summary>
  public class MemoryModel : ObservablePoint
  {
    /// <summary>
    /// Date time of item
    /// </summary>
    public DateTime Date
    {
      get;
      set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    public MemoryModel(double x, double y)
      : base(x, y)
    {
    }
  }
}
