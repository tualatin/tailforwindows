using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data.Enums;

namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data.Mappings
{
  /// <summary>
  /// Analysis of mapping
  /// </summary>
  public class AnalysisOfMapping
  {
    /// <summary>
    /// Analysis of as enum <see cref="EAnalysisOf"/>
    /// </summary>
    public EAnalysisOf AnalysisOf
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description
    {
      get
      {
        try
        {
          var resourceKey = Application.Current.TryFindResource($"Analysis_{AnalysisOf.ToString()}");

          return resourceKey?.ToString() ?? string.Empty;
        }
        catch
        {
          return string.Empty;
        }

      }
    }
  }
}
