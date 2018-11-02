using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using log4net;
using Org.Vs.TailForWin.Business.StatisticEngine.Controllers;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data;
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
    private IStatisticAnalysisCollection _statisticAnalysisCollection;

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

    private AnalysisOfMapping _currentAnalysisOf;

    /// <summary>
    /// Current <see cref="AnalysisOfMapping"/> selection
    /// </summary>
    public AnalysisOfMapping CurrentAnalysisOf
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

    #endregion

    #region Memory usage chart

    /// <summary>
    /// Memory usage series
    /// </summary>
    public SeriesCollection MemoryUsageSeries
    {
      get;
      set;
    }

    public Func<double, string> Formatter
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

      PaintChartDiagram();
    }

    private void PaintChartDiagram()
    {
      if ( _statisticAnalysisCollection == null || _statisticAnalysisCollection.Count == 0 )
        return;

      CreateMemoryUsageChart();
    }

    private void CreateMemoryUsageChart()
    {
      var dayConfig = Mappers.Xy<DateModel>().X(p => p.Value).Y(p => (double) p.TimeSpan.Ticks / TimeSpan.FromMinutes(15).Ticks);
      var memoryUsage = new ChartValues<double>();
      var upSessionUptime = new ChartValues<DateModel>();
      MemoryUsageSeries = new SeriesCollection(dayConfig)
      {
        new ColumnSeries
        {
          Values = upSessionUptime,
          DataLabels = true,
          LabelPoint = MemoryUsageLabelPoint
        },
        new LineSeries
        {
          Values = memoryUsage,
          LineSmoothness = 1,
          PointGeometrySize = 8,
          StrokeThickness = 2,
          Fill = Brushes.Transparent
        }
      };

      var upTime = new TimeSpan();
      var count = 0;

      foreach ( StatisticAnalysisData item in _statisticAnalysisCollection )
      {
        memoryUsage.Add(Math.Round((item.SessionEntity.MemoryUsage / 1024d) / 1014, 2));
        upSessionUptime.Add(new DateModel
        {
          TimeSpan = item.SessionEntity.UpTime,
          Value = count
        });
        upTime = upTime.Add(item.SessionEntity.UpTime);
        count++;
      }

      UpTime = $"{Application.Current.TryFindResource("AnalysisTotalUpTime")} {upTime.Days:D0} {Application.Current.TryFindResource("AboutUptimeDays")} " +
               $"{upTime.Hours:D2}:{upTime.Minutes:D2}:{upTime.Seconds:D2} {Application.Current.TryFindResource("AboutUptimeHours")}";
      Formatter = MemoryUsageFormatter;

      OnPropertyChanged(nameof(MemoryUsageSeries));
    }

    private string MemoryUsageFormatter(double arg)
    {
      double day = arg + 1;
      string plural = (int) day == 1 ? Application.Current.TryFindResource("AnalysisMemUsageDay").ToString() : Application.Current.TryFindResource("AnalysisMemUsageDays").ToString();
      return $"{day} {plural}";
    }

    private string MemoryUsageLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is DateModel model) )
        return string.Empty;

      var result = new StringBuilder();
      result.AppendLine($"{model.TimeSpan.Days:D0}{Application.Current.TryFindResource("AnalysisMemUsageDaysShort")}");
      result.Append($"{model.TimeSpan.Hours:D2}:{model.TimeSpan.Minutes:D2} {Application.Current.TryFindResource("AnalysisMemUsageHoursShort")}");

      return result.ToString();
    }
  }
}
