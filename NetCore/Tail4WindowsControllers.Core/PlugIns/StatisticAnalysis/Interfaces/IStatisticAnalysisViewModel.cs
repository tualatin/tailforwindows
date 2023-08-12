using System.Windows.Input;
using Org.Vs.Tail4Win.Business.StatisticEngine.Data;
using Org.Vs.Tail4Win.Business.StatisticEngine.Interfaces;
using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;
using Org.Vs.Tail4Win.Controllers.PlugIns.StatisticAnalysis.Data.Enums;
using Org.Vs.Tail4Win.Controllers.PlugIns.StatisticAnalysis.Data.Mappings;
using Org.Vs.Tail4Win.Controllers.UI.Interfaces;
using Org.Vs.Tail4Win.Core.Collections;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.StatisticAnalysis.Interfaces
{
  /// <summary>
  /// StatisticAnalysis view model interface
  /// </summary>
  public interface IStatisticAnalysisViewModel : IViewModelBase
  {
    #region Properties

    /// <summary>
    /// Statistic analysis collection
    /// </summary>
    IStatisticAnalysisCollection<StatisticAnalysisData> StatisticAnalysisCollection

    {
      get;
    }

    /// <summary>
    /// Top
    /// </summary>
    double Top
    {
      get;
      set;
    }

    /// <summary>
    /// Left
    /// </summary>
    double Left
    {
      get;
      set;
    }

    /// <summary>
    /// Width
    /// </summary>
    double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Height
    /// </summary>
    double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Up time
    /// </summary>
    string UpTime
    {
      get;
    }

    /// <summary>
    /// Total files
    /// </summary>
    string TotalFiles
    {
      get;
    }

    /// <summary>
    /// Analysis of mappings of type <see cref="AnalysisOfMapping"/>
    /// </summary>
    AsyncObservableCollection<AnalysisOfMapping> AnalysisOfMappings
    {
      get;
      set;
    }

    /// <summary>
    /// Current <see cref="AnalysisOfMapping"/> selection
    /// </summary>
    EAnalysisOf CurrentAnalysisOf
    {
      get;
      set;
    }

    /// <summary>
    /// Is busy
    /// </summary>
    bool IsBusy
    {
      get;
    }

    #endregion

    #region Commands

    /// <summary>
    /// Closing command
    /// </summary>
    ICommand ClosingCommand
    {
      get;
    }

    /// <summary>
    /// Refresh command
    /// </summary>
    IAsyncCommand RefreshCommand
    {
      get;
    }

    #endregion
  }
}
