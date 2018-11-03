using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.StatisticEngine.Controllers;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data.Enums;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data.Mappings;
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

    private string _upTime;

    /// <summary>
    /// Up time
    /// </summary>
    public string UpTime
    {
      get => _upTime;
      set
      {
        if ( Equals(value, _upTime) )
          return;

        _upTime = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Analysis of mappings of type <see cref="AnalysisOfMapping"/>
    /// </summary>
    public AsyncObservableCollection<AnalysisOfMapping> AnalysisOfMappings
    {
      get;
      set;
    }

    private EAnalysisOf _currentAnalysisOf;

    /// <summary>
    /// Current <see cref="AnalysisOfMapping"/> selection
    /// </summary>
    public EAnalysisOf CurrentAnalysisOf
    {
      get => _currentAnalysisOf;
      set
      {
        if ( Equals(value, _currentAnalysisOf) )
          return;

        _currentAnalysisOf = value;
        OnPropertyChanged();
      }
    }

    private IStatisticAnalysisCollection _statisticAnalysisCollection;

    /// <summary>
    /// Statistic analysis collection
    /// </summary>
    public IStatisticAnalysisCollection StatisticAnalysisCollection
    {
      get => _statisticAnalysisCollection;
      set
      {
        _statisticAnalysisCollection = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StatisticAnalysisViewModel()
    {
      _statisticController = new StatisticController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnLoadedPropertyChanged;
      ((AsyncCommand<object>) RefreshCommand).PropertyChanged += OnRefreshPropertyChanged;

      var analysisOf = new AsyncObservableCollection<AnalysisOfMapping>();

      foreach ( EAnalysisOf analysis in Enum.GetValues(typeof(EAnalysisOf)) )
      {
        analysisOf.Add(new AnalysisOfMapping
        {
          AnalysisOf = analysis
        });
      }

      AnalysisOfMappings = new AsyncObservableCollection<AnalysisOfMapping>(analysisOf);
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

    private IAsyncCommand _refreshCommand;

    /// <summary>
    /// Refresh command
    /// </summary>
    public IAsyncCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = AsyncCommand.Create(ExecuteRefreshCommandAsync));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand
    {
      get;
    }

    #endregion

    #region Command functions

    private async Task ExecuteRefreshCommandAsync() => await CalculationStatisticsAsync();

    private async Task ExecuteLoadedCommandAsync() => await CalculationStatisticsAsync();

    private void ExecuteClosingCommand()
    {
      SettingsHelperController.CurrentSettings.StatisticWindowLeft = Left;
      SettingsHelperController.CurrentSettings.StatisticWindowTop = Top;

      SettingsHelperController.CurrentSettings.StatisticWindowHeight = Height;
      SettingsHelperController.CurrentSettings.StatisticWindowWidth = Width;

      _cts?.Cancel();
    }

    #endregion

    private async Task CalculationStatisticsAsync()
    {
      MouseService.SetBusyState();

      try
      {
        _cts?.Dispose();
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        _statisticAnalysisCollection = await _statisticController.StartAnalysisAsync(_cts.Token).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void MoveIntoView()
    {
      double posX = SettingsHelperController.CurrentSettings.StatisticWindowLeft;
      double posY = SettingsHelperController.CurrentSettings.StatisticWindowTop;

      UiHelper.MoveIntoView(Application.Current.TryFindResource("ExtrasStatistics").ToString(), ref posX, ref posY, SettingsHelperController.CurrentSettings.StatisticWindowWidth,
        SettingsHelperController.CurrentSettings.StatisticWindowHeight);

      SettingsHelperController.CurrentSettings.StatisticWindowLeft = posX;
      SettingsHelperController.CurrentSettings.StatisticWindowTop = posY;
    }

    private void OnRefreshPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      OnPropertyChanged(nameof(StatisticAnalysisCollection));
      PaintChartDiagram();
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

      OnPropertyChanged(nameof(StatisticAnalysisCollection));
      PaintChartDiagram();
    }

    private void PaintChartDiagram()
    {
      if ( _statisticAnalysisCollection == null || _statisticAnalysisCollection.Count == 0 )
        return;

      var upTime = new TimeSpan();

      foreach ( StatisticAnalysisData item in _statisticAnalysisCollection )
      {
        upTime = upTime.Add(item.SessionEntity.UpTime);
      }

      UpTime = $"{Application.Current.TryFindResource("AnalysisTotalUpTime")} {upTime.Days:D0} {Application.Current.TryFindResource("AboutUptimeDays")} " +
               $"{upTime.Hours:D2}:{upTime.Minutes:D2}:{upTime.Seconds:D2} {Application.Current.TryFindResource("AboutUptimeHours")}";
    }
  }
}
