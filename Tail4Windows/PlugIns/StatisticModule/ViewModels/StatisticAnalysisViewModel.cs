using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using log4net;
using Org.Vs.TailForWin.Business.StatisticEngine.Controllers;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Vml.Attributes;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Utils;


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
    private CancellationTokenSource _cts;

    #region Properties

    private double _top;

    /// <summary>
    /// Top
    /// </summary>
    public double Top
    {
      get => _top;
      set
      {
        _top = value;
        OnPropertyChanged();
      }
    }

    private double _left;

    /// <summary>
    /// Left
    /// </summary>
    public double Left
    {
      get => _left;
      set
      {
        _left = value;
        OnPropertyChanged();
      }
    }

    private double _width;

    /// <summary>
    /// Width
    /// </summary>
    public double Width
    {
      get => _width;
      set
      {
        _width = value;
        OnPropertyChanged();
      }
    }

    private double _height;

    /// <summary>
    /// Height
    /// </summary>
    public double Height
    {
      get => _height;
      set
      {
        _height = value;
        OnPropertyChanged();
      }
    }

    public SeriesCollection Series
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StatisticAnalysisViewModel()
    {
      _statisticController = new StatisticController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnLoadedPropertyChanged;

      Series = new SeriesCollection
      {
        new LineSeries
        {
          Values = new ChartValues<double> { 3, 5, 7, 4 }
        },
        new ColumnSeries
        {
          Values = new ChartValues<decimal> { 5, 6, 2, 7 }
        }
      };
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    private ICommand _closingCommand;

    /// <summary>
    /// Closing command
    /// </summary>
    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand(p => ExecuteClosingCommand()));

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
        _cts?.Dispose();
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        await _statisticController.StartAnalysisAsync(_cts.Token).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void ExecuteClosingCommand()
    {
      SettingsHelperController.CurrentSettings.StatisticWindowLeft = Left;
      SettingsHelperController.CurrentSettings.StatisticWindowTop = Top;

      SettingsHelperController.CurrentSettings.StatisticWindowHeight = Height;
      SettingsHelperController.CurrentSettings.StatisticWindowWidth = Width;

      _cts?.Cancel();
    }

    #endregion

    private void MoveIntoView()
    {
      double posX = SettingsHelperController.CurrentSettings.StatisticWindowLeft;
      double posY = SettingsHelperController.CurrentSettings.StatisticWindowTop;

      UiHelper.MoveIntoView(Application.Current.TryFindResource("ExtrasStatistics").ToString(), ref posX, ref posY, SettingsHelperController.CurrentSettings.StatisticWindowWidth,
        SettingsHelperController.CurrentSettings.StatisticWindowHeight);

      SettingsHelperController.CurrentSettings.StatisticWindowLeft = posX;
      SettingsHelperController.CurrentSettings.StatisticWindowTop = posY;
    }

    private void OnLoadedPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      MoveIntoView();

      Left = SettingsHelperController.CurrentSettings.StatisticWindowLeft;
      Top = SettingsHelperController.CurrentSettings.StatisticWindowTop;

      Height = SettingsHelperController.CurrentSettings.StatisticWindowHeight;
      Width = SettingsHelperController.CurrentSettings.StatisticWindowWidth;
    }
  }
}
