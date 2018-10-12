using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.StatisticEngine.Controllers;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Vml.Attributes;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.ViewModels
{
  /// <summary>
  /// StatisticAnalysis view model
  /// </summary>
  [Locator(nameof(StatisticAnalysisViewModel))]
  public class StatisticAnalysisViewModel : NotifyMaster, IStatisticAnalysisViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(StatisticAnalysisViewModel));

    private readonly IStatisticController _statisticController;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StatisticAnalysisViewModel()
    {
      _statisticController = new StatisticController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnLoadedPropertyChanged;
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand
    {
      get;
    }

    #endregion

    #region Command functions

    private async Task ExecuteLoadedCommandAsync()
    {
      MouseService.SetBusyState();

      try
      {
        await _statisticController.StartAnalysisAsync().ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    #endregion

    private void OnLoadedPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;
    }
  }
}
