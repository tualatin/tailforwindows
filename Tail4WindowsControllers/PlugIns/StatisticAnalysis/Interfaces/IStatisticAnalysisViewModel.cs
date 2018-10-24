using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


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
