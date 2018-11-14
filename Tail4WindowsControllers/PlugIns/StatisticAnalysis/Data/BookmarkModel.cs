using LiveCharts.Defaults;


namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data
{
  /// <summary>
  /// Bookmark model
  /// </summary>
  public class BookmarkModel : ObservablePoint
  {
    /// <summary>
    /// Bookmark count
    /// </summary>
    public ulong BookmarkCount
    {
      get;
      set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    public BookmarkModel(double x, double y)
      : base(x, y)
    {
    }
  }
}
