using LiveCharts.Defaults;

namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data
{
  /// <summary>
  /// File size model
  /// </summary>
  public class FileSizeModel : ObservablePoint
  {
    /// <summary>
    /// Is Windows event
    /// </summary>
    public bool IsWindowsEvent
    {
      get;
      set;
    }

    /// <summary>
    /// File size
    /// </summary>
    public double FileSize
    {
      get;
      set;
    }

    /// <summary>
    /// Log count
    /// </summary>
    public ulong LogCount
    {
      get;
      set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    public FileSizeModel(double x, double y)
      : base(x, y)
    {
    }
  }
}
