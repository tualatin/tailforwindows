using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls
{
  /// <summary>
  /// Interaction logic for UCMemoryUsageChart.xaml
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public partial class UCMemoryUsageChart : IUcMemoryUsageChart
  {
    private NotifyTaskCompletion _runner;

    /// <summary>
    /// Chart series
    /// </summary>
    public SeriesCollection ChartSeries
    {
      get;
      set;
    }

    private string _averageRunningTime;

    /// <summary>
    /// Average running time
    /// </summary>
    public string AverageRunningTime
    {
      get => _averageRunningTime;
      set
      {
        if ( Equals(value, _averageRunningTime) )
          return;

        _averageRunningTime = value;
        OnPropertyChanged();
      }
    }

    private string _minMemoryUsage;

    /// <summary>
    /// Minimum memory usage
    /// </summary>
    public string MinMemoryUsage
    {
      get => _minMemoryUsage;
      set
      {
        if ( Equals(value, _minMemoryUsage) )
          return;

        _minMemoryUsage = value;
        OnPropertyChanged();
      }
    }

    private string _averageMemoryUsage;

    /// <summary>
    /// Average memory usage
    /// </summary>
    public string AverageMemoryUsage
    {
      get => _averageMemoryUsage;
      set
      {
        if ( Equals(value, _averageMemoryUsage) )
          return;

        _averageMemoryUsage = value;
        OnPropertyChanged();
      }
    }

    private string _maxMemoryUsage;

    /// <summary>
    /// Maximum memory usage
    /// </summary>
    public string MaxMemoryUsage
    {
      get => _maxMemoryUsage;
      set
      {
        if ( Equals(value, _maxMemoryUsage) )
          return;

        _maxMemoryUsage = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// XAxis formatter function
    /// </summary>
    public Func<double, string> XAxisFormatter
    {
      get;
      set;
    }

    /// <summary>
    /// YAxis formatter function
    /// </summary>
    public Func<double, string> YAxisFormatter
    {
      get;
      set;
    }

    #region Dependency properties

    /// <summary>
    /// Analysis collection property <see cref="IStatisticAnalysisCollection{T}"/>
    /// </summary>
    public static readonly DependencyProperty AnalysisCollectionProperty = DependencyProperty.Register(nameof(AnalysisCollection),
      typeof(IStatisticAnalysisCollection<StatisticAnalysisData>), typeof(UCMemoryUsageChart), new PropertyMetadata(null, OnAnalysisCollectionChanged));

    private static void OnAnalysisCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is UCMemoryUsageChart myChart) )
        return;

      myChart._runner = NotifyTaskCompletion.Create(myChart.CreateChartAsync());
      myChart._runner.PropertyChanged += myChart.OnRunnerPropertyChanged;
    }

    private void OnRunnerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      XAxisFormatter = MemoryUsageXAxisFormatter;
      YAxisFormatter = MemoryUsageYAxisFormatter;

      OnPropertyChanged(nameof(ChartSeries));
      OnPropertyChanged(nameof(AverageMemoryUsage));
      OnPropertyChanged(nameof(AverageRunningTime));
      OnPropertyChanged(nameof(MinMemoryUsage));
      OnPropertyChanged(nameof(MaxMemoryUsage));

      if ( _runner == null )
        return;

      _runner.PropertyChanged -= OnRunnerPropertyChanged;
      _runner = null;
    }

    /// <summary>
    /// Analysis collection
    /// </summary>
    public IStatisticAnalysisCollection<StatisticAnalysisData> AnalysisCollection
    {
      get => (IStatisticAnalysisCollection<StatisticAnalysisData>) GetValue(AnalysisCollectionProperty);
      set => SetValue(AnalysisCollectionProperty, value);
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public UCMemoryUsageChart()
    {
      InitializeComponent();
      DataContext = this;
    }

    #region Commands

    private ICommand _resetViewCommand;

    /// <summary>
    /// Reset current view settings (zoom, panning...)
    /// </summary>
    public ICommand ResetViewCommand => _resetViewCommand ?? (_resetViewCommand = new RelayCommand(p => CanResetView(), p => ExecuteResetViewCommand()));

    private ICommand _updaterTickCommand;

    /// <summary>
    /// Updater tick command
    /// </summary>
    public ICommand UpdaterTickCommand => _updaterTickCommand ?? (_updaterTickCommand = new RelayCommand(p => ExecuteUpdaterTickCommand((CartesianChart) p)));

    #endregion

    #region Command functions

    private void ExecuteUpdaterTickCommand(CartesianChart e)
    {
    }

    /// <summary>
    /// Can reset current view
    /// </summary>
    /// <returns>If command can execute <c>True</c> otherwise <c>False</c></returns>
    public bool CanResetView() => AnalysisCollection.Count != 0;

    private void ExecuteResetViewCommand()
    {
      XAxis.MinValue = double.NaN;
      XAxis.MaxValue = double.NaN;
      YAxis.MinValue = 0;
      YAxis.MaxValue = double.NaN;
    }

    #endregion

    /// <summary>
    /// Create a chart view async
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    public async Task CreateChartAsync()
    {
      var dayConfig = Mappers.Xy<DateModel>().X(p => p.Value).Y(p => (double) p.TimeSpan.Ticks / TimeSpan.FromMinutes(15).Ticks);
      var memoryConfig = Mappers.Xy<MemoryModel>().X(p => p.X).Y(p => p.Y);

      var memoryUsage = new ChartValues<MemoryModel>();
      var upSessionUptime = new ChartValues<DateModel>();
      ChartSeries = new SeriesCollection
      {
        new ColumnSeries(dayConfig)
        {
          Values = upSessionUptime,
          LabelPoint = UpTimeLabelPoint,
          Title = Application.Current.TryFindResource("AnalysisMemUsageUpTime").ToString()
        },
        new LineSeries(memoryConfig)
        {
          Values = memoryUsage,
          LineSmoothness = 1,
          PointGeometrySize = 8,
          StrokeThickness = 2,
          LabelPoint = MemoryUsageLabelPoint,
          Fill = Brushes.Transparent,
          Title = Application.Current.TryFindResource("AnalysisMemUsageMemory").ToString()
        }
      };

      var analysisCollection = AnalysisCollection;

      await Task.Run(() =>
      {
        if ( analysisCollection.Count == 0 )
        {
          _averageMemoryUsage = string.Empty;
          _minMemoryUsage = string.Empty;
          _maxMemoryUsage = string.Empty;
          return;
        }

        var count = 0;

        foreach ( StatisticAnalysisData item in analysisCollection )
        {
          memoryUsage.Add(new MemoryModel(count, Math.Round((item.SessionEntity.MemoryUsage / 1024d) / 1014, 2))
          {
            Date = item.SessionEntity.Date,
            FileCount = item.Files.Count
          });

          upSessionUptime.Add(new DateModel
          {
            TimeSpan = item.SessionEntity.UpTime,
            Date = item.SessionEntity.Date,
            Value = count
          });
          count++;
        }

        var averageMem = memoryUsage.Average(p => p.Y);
        var averageRunningTime = (long) upSessionUptime.Average(p => p.TimeSpan.Ticks);

        if ( averageRunningTime.Equals(0) )
        {
          _averageRunningTime = string.Empty;
        }
        else
        {
          var timeSpan = new TimeSpan(averageRunningTime);
          _averageRunningTime = $"{timeSpan.Days:D0} {Application.Current.TryFindResource("AboutUptimeDays")} " +
                                $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2} {Application.Current.TryFindResource("AboutUptimeHours")}";
        }

        _averageMemoryUsage = $"{averageMem:N2}";
        _minMemoryUsage = $"{memoryUsage.Min(p => p.Y):N2}";
        _maxMemoryUsage = $"{memoryUsage.Max(p => p.Y):N2}";
      }).ConfigureAwait(false);
    }

    private string MemoryUsageLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is MemoryModel model) )
        return string.Empty;

      var series = ChartSeries.Where(p => p.IsSeriesVisible).ToList();
      string file = model.FileCount == 1 ?
        Application.Current.TryFindResource("AnalysisMemUsageFile").ToString() :
        Application.Current.TryFindResource("AnalysisMemUsageFiles").ToString();

      if ( series.Count == 2 )
        return $"{model.Y} MB ({model.FileCount} {file})";

      var result = new StringBuilder();
      result.AppendLine($"{model.Date.ToString(SettingsHelperController.CurrentSettings.CurrentDateFormat)}");
      result.Append($"{model.Y} MB ({model.FileCount} {file})");

      return result.ToString();
    }

    private string MemoryUsageXAxisFormatter(double arg)
    {
      double session = arg + 1;
      return $"{session}.";
    }

    private string MemoryUsageYAxisFormatter(double arg) => $"{arg} MB";

    private string UpTimeLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is DateModel model) )
        return string.Empty;

      var result = new StringBuilder();
      result.AppendLine($"{model.Date.ToString(SettingsHelperController.CurrentSettings.CurrentDateFormat)}");
      result.Append($"{model.TimeSpan.Days:D0}{Application.Current.TryFindResource("AnalysisMemUsageDaysShort")} ");
      result.Append($"{model.TimeSpan.Hours:D2}:{model.TimeSpan.Minutes:D2} {Application.Current.TryFindResource("AnalysisMemUsageHoursShort")}");

      return result.ToString();
    }

    private void OnListBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if ( !(ItemsControl.ContainerFromElement(ListBox, (DependencyObject) e.OriginalSource) is ListBoxItem item) )
        return;

      var series = (Series) item.Content;
      series.Visibility = series.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
