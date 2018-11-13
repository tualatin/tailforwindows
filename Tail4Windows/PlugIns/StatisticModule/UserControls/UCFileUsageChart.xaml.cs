using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.DbScheme;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls
{
  /// <summary>
  /// Interaction logic for UCFileUsageChart.xaml
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public partial class UCFileUsageChart : IUcFileUsageChart
  {
    private NotifyTaskCompletion _runner;

    #region Properties

    /// <summary>
    /// Chart series
    /// </summary>
    public SeriesCollection ChartSeries
    {
      get;
      set;
    }

    private string _totalLinesRead;

    /// <summary>
    /// Total lines read
    /// </summary>
    public string TotalLinesRead
    {
      get => _totalLinesRead;
      set
      {
        if ( Equals(value, _totalLinesRead) )
          return;

        _totalLinesRead = value;
        OnPropertyChanged();
      }
    }

    private string _averageDailyFileCount;

    /// <summary>
    /// Average daily file count
    /// </summary>
    public string AverageDailyFileCount
    {
      get => _averageDailyFileCount;
      set
      {
        if ( Equals(value, _averageDailyFileCount) )
          return;

        _averageDailyFileCount = value;
        OnPropertyChanged();
      }
    }

    private string _averageLogCount;

    /// <summary>
    /// Average log count
    /// </summary>
    public string AverageLogCount
    {
      get => _averageLogCount;
      set
      {
        if ( Equals(value, _averageLogCount) )
          return;

        _averageLogCount = value;
        OnPropertyChanged();
      }
    }

    private string _averageDailyLogCount;

    /// <summary>
    /// Average daily log count
    /// </summary>
    public string AverageDailyLogCount
    {
      get => _averageDailyLogCount;
      set
      {
        if ( Equals(value, _averageDailyLogCount) )
          return;

        _averageDailyLogCount = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public UCFileUsageChart()
    {
      InitializeComponent();
      DataContext = this;
    }

    #region Dependency properties

    /// <summary>
    /// Analysis collection property <see cref="IStatisticAnalysisCollection{T}"/>
    /// </summary>
    public static readonly DependencyProperty AnalysisCollectionProperty = DependencyProperty.Register(nameof(AnalysisCollection),
      typeof(IStatisticAnalysisCollection<StatisticAnalysisData>), typeof(UCFileUsageChart), new PropertyMetadata(null, OnAnalysisCollectionChanged));

    private static void OnAnalysisCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is UCFileUsageChart myChart) )
        return;

      myChart._runner = NotifyTaskCompletion.Create(myChart.CreateChartAsync());
      myChart._runner.PropertyChanged += myChart.OnRunnerPropertyChanged;
    }

    private void OnRunnerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      XAxisFormatter = FileUsageXAxisFormatter;
      YAxisFormatter = FileUsageYAxisFormatter;

      OnPropertyChanged(nameof(ChartSeries));
      OnPropertyChanged(nameof(TotalLinesRead));
      OnPropertyChanged(nameof(AverageDailyFileCount));
      OnPropertyChanged(nameof(AverageLogCount));
      OnPropertyChanged(nameof(AverageDailyLogCount));

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
      //XAxis.MinValue = double.NaN;
      //XAxis.MaxValue = double.NaN;
      //YAxis.MinValue = 0;
      //YAxis.MaxValue = double.NaN;
    }

    #endregion

    /// <summary>
    /// Create a chart view async
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    public async Task CreateChartAsync()
    {
      var analysisCollection = AnalysisCollection;

      Task totalLines = CalcTotalLinesReadAsync(analysisCollection);
      Task averageDailyFileCount = CalcAverageDailyFileCountAsync(analysisCollection);
      Task createChartView = CreateChartViewAsync(analysisCollection);
      Task createPieChartView = CreatePieChartViewAsync(analysisCollection);

      await Task.WhenAll(totalLines, averageDailyFileCount, createChartView, createPieChartView).ConfigureAwait(false);
    }

    private async Task CreatePieChartViewAsync(IStatisticAnalysisCollection<StatisticAnalysisData> collection)
    {
      var defaultLogFile = new ChartValues<int>();
      var smartWatchLogFile = new ChartValues<int>();
      var windowEvent = new ChartValues<int>();
      ChartSeries = new SeriesCollection
      {
        new PieSeries
        {
          DataLabels = true,
          Values = defaultLogFile,
          Title = Application.Current.TryFindResource("AnalysisFileUsageLogFiles").ToString(),
          LabelPoint = PieChartLabelPoint
        },
        new PieSeries
        {
          DataLabels = true,
          Values = smartWatchLogFile,
          Title = Application.Current.TryFindResource("AnalysisFileUsageLogFilesIsSmartWatch").ToString(),
          LabelPoint = PieChartLabelPoint
        },
        new PieSeries
        {
          DataLabels = true,
          Values = windowEvent,
          Title = Application.Current.TryFindResource("AnalysisFileUsageWindowsEventLog").ToString(),
          LabelPoint = PieChartLabelPoint
        }
      };

      await Task.Run(() =>
      {
        var logFiles = new List<FileEntity>();
        var windowsEvents = new List<FileEntity>();

        foreach ( var item in collection )
        {
          logFiles.AddRange(item.Files.Where(p => !p.IsWindowsEvent).ToList());
          windowsEvents.AddRange(item.Files.Where(p => p.IsWindowsEvent).ToList());
        }

        int isSmartWatch = logFiles.Where(p => p.IsSmartWatch).ToList().Count;

        defaultLogFile.Add(logFiles.Count - isSmartWatch);
        smartWatchLogFile.Add(isSmartWatch);
        windowEvent.Add(windowsEvents.Count);
      }).ConfigureAwait(false);
    }

    private async Task CreateChartViewAsync(IStatisticAnalysisCollection<StatisticAnalysisData> collection)
    {
      var fileSessionConfig = Mappers.Xy<FileSessionModel>().X(p => p.SessionCount).Y(p => (double) p.TimeSpan.Ticks / TimeSpan.FromMinutes(15).Ticks);
      ChartSeries = new SeriesCollection();
      var count = 0;

      foreach ( StatisticAnalysisData item in collection )
      {
        var fileSessions = new ChartValues<FileSessionModel>();
        var columnSeries = new ColumnSeries(fileSessionConfig)
        {
          //LabelPoint = FileSessionLabelPoint,
          Values = fileSessions,
          Title = item.SessionEntity.Session.ToString()
        };

        foreach ( FileEntity file in item.Files )
        {
          var model = new FileSessionModel
          {
            Date = item.SessionEntity.Date,
            SessionCount = count,
            BookmarkCount = file.BookmarkCount,
            FileSize = file.FileSizeTotalEvents,
            IsSmartWatch = file.IsSmartWatch,
            IsWindowsEvent = file.IsWindowsEvent,
            LogCount = file.LogCount
          };

          if ( file.ElapsedTime.HasValue )
            model.TimeSpan = file.ElapsedTime.Value;

          fileSessions.Add(model);
        }

        count++;
        ChartSeries.Add(columnSeries);

        if ( count == 5 )
          break;
      }
    }

    private async Task CalcTotalLinesReadAsync(IStatisticAnalysisCollection<StatisticAnalysisData> collection) => await Task.Run(() =>
    {
      if ( collection.Count == 0 )
      {
        _totalLinesRead = string.Empty;
        _averageLogCount = string.Empty;
        return;
      }

      ulong lines = 0;
      var files = 0;

      foreach ( StatisticAnalysisData item in collection )
      {
        ulong current = 0;
        current = item.Files.Aggregate(current, (c, i) => c + i.LogCount);
        lines += current;
        files += item.Files.Count;
      }

      _totalLinesRead = $"{lines:N0}";
      _averageLogCount = $"{(decimal) lines / files:N2}";
    }).ConfigureAwait(false);


    private async Task CalcAverageDailyFileCountAsync(IStatisticAnalysisCollection<StatisticAnalysisData> collection) => await Task.Run(() =>
    {
      if ( collection.Count == 0 )
      {
        _averageDailyFileCount = string.Empty;
        _averageDailyLogCount = string.Empty;
        return;
      }

      var result = collection.GroupBy(p => p.SessionEntity.Date).Select(p => p.Select(x => x.Files)).ToList();

      if ( result.Count == 0 )
        return;

      var fileCount = 0;
      decimal dailyAverage = decimal.Zero;

      foreach ( var item in result )
      {
        int currentFileCount = item.Select(p => p.Count).FirstOrDefault();
        ulong currentLinesCount = 0;
        currentLinesCount = item.Select(p => p.Aggregate(currentLinesCount, (c, i) => c + i.LogCount)).FirstOrDefault();

        dailyAverage += (decimal) currentLinesCount / currentFileCount;
        fileCount += currentFileCount;
      }

      _averageDailyFileCount = $"{(decimal) fileCount / result.Count:N1}";
      _averageDailyLogCount = $"{dailyAverage / result.Count:N2}";
    }).ConfigureAwait(false);

    private string PieChartLabelPoint(ChartPoint arg) => $"{arg.Y} ({arg.Participation:P})";

    private string FileSessionLabelPoint(ChartPoint arg)
    {
      // TODO Label
      return string.Empty;
    }

    private string FileUsageXAxisFormatter(double arg)
    {
      double session = arg + 1;
      return $"{session}.";
    }

    private string FileUsageYAxisFormatter(double arg)
    {
      return arg.ToString();
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

    private void OnListBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {

    }

    private void OnPieChartDataClick(object sender, ChartPoint e)
    {
      var chart = (PieChart) e.ChartView;

      foreach ( var seriesView in chart.Series )
      {
        var series = (PieSeries) seriesView;
        series.PushOut = 0;
      }

      var selectedSeries = (PieSeries) e.SeriesView;
      selectedSeries.PushOut = 10;
    }
  }
}
