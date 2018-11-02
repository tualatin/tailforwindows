using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data.Mappings;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Interfaces
{
  /// <summary>
  /// StatisticAnalysis view model interface
  /// </summary>
  public interface IStatisticAnalysisViewModel : IViewModelBase
  {
    #region Properties

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
      set;
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
    AnalysisOfMapping CurrentAnalysisOf
    {
      get;
      set;
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
