using System;
using System.Collections.Generic;
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
using Org.Vs.TailForWin.Business.StatisticEngine.Data.Extensions;
using Org.Vs.TailForWin.Business.StatisticEngine.DbScheme;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data;
using Org.Vs.TailForWin.Core.Controllers;
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

    /// <summary>
    /// PieChart series
    /// </summary>
    public SeriesCollection PieSeries
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

    /// <summary>
    /// Chart visibility
    /// </summary>
    public List<ChartVisibility> ChartVisibility
    {
      get;
      set;
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
      ChartVisibility = new List<ChartVisibility>
      {
        new ChartVisibility
        {
          Visibility = Visibility.Visible,
          Title = Application.Current.TryFindResource("AnalysisFileUsagePieChart").ToString()
        },
        new ChartVisibility
        {
          Visibility = Visibility.Collapsed,
          Title = Application.Current.TryFindResource("AnalysisFileUsageColumnChart").ToString()
        }
      };

      OnPropertyChanged(nameof(ChartVisibility));
      OnPropertyChanged(nameof(ChartSeries));
      OnPropertyChanged(nameof(PieSeries));
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
      PieSeries = new SeriesCollection
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
      var fileSessionConfig = Mappers.Xy<FileSessionModel>()
        .X(p => p.FilesCount)
        .Y(p => (double) p.TimeSpan.Ticks / TimeSpan.FromMinutes(15).Ticks)
        .Fill(p => p.IsWindowsEvent ? Brushes.DarkOrange : p.IsSmartWatch ? Brushes.DarkOrchid : Brushes.DodgerBlue);
      var bookmarkConfig = Mappers.Xy<BookmarkModel>().X(p => p.X).Y(p => p.Y);
      var fileSizeConfig = Mappers.Xy<FileSizeModel>().X(p => p.X).Y(p => p.Y);

      var fileValues = new ChartValues<FileSessionModel>();
      var bookmarkValues = new ChartValues<BookmarkModel>();
      var fileSize = new ChartValues<FileSizeModel>();
      ChartSeries = new SeriesCollection
      {
        new ColumnSeries(fileSessionConfig)
        {
          Values = fileValues,
          LabelPoint = FileSessionLabelPoint,
          Title = Application.Current.TryFindResource("AnalysisMemUsageFiles").ToString()
        },
        new LineSeries(bookmarkConfig)
        {
          Values = bookmarkValues,
          LabelPoint = BookmarkLabelPoint,
          Title = Application.Current.TryFindResource("AnalysisFileUsageBookmarkTitle").ToString()
        },
        new LineSeries(fileSizeConfig)
        {
          Values = fileSize,
          LabelPoint = FileSizeLabelPoint,
          Title = Application.Current.TryFindResource("AnalysisFileUsageFileSizeTitle").ToString()
        }
      };

      await Task.Run(() =>
      {
        var count = 0;
        var sessionCount = 1;

        // Get only file entries and select the max file size
        var maxFileSize = collection.GetMaxFileSize();

        // Get only Windows events and select the max number of events
        var maxWindowsEvents = collection.GetMaxWindowsEventSize();
        double maxValue = maxFileSize / 100;

        foreach ( StatisticAnalysisData item in collection )
        {
          foreach ( FileEntity file in item.Files )
          {
            var sessionModel = new FileSessionModel
            {
              FilesCount = count,
              SessionCount = sessionCount,
              BookmarkCount = file.BookmarkCount,
              FileSize = file.FileSizeTotalEvents,
              LogCount = file.LogCount,
              IsSmartWatch = file.IsSmartWatch,
              IsWindowsEvent = file.IsWindowsEvent,
              Date = item.SessionEntity.Date
            };

            if ( file.ElapsedTime.HasValue )
              sessionModel.TimeSpan = file.ElapsedTime.Value;

            fileValues.Add(sessionModel);
            bookmarkValues.Add(new BookmarkModel(count, (double) file.BookmarkCount / 2)
            {
              BookmarkCount = file.BookmarkCount
            });

            double value = file.IsWindowsEvent ?
              file.FileSizeTotalEvents.Equals(maxWindowsEvents) ?
                maxValue :
                (file.FileSizeTotalEvents * maxValue) / maxWindowsEvents :
              file.FileSizeTotalEvents / 100;

            fileSize.Add(new FileSizeModel(count, value)
            {
              IsWindowsEvent = file.IsWindowsEvent,
              FileSize = file.FileSizeTotalEvents,
              LogCount = file.LogCount
            });
            count++;
          }

          sessionCount++;
        }
      }).ConfigureAwait(false);
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

    private string BookmarkLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is BookmarkModel model) )
        return string.Empty;

      var text = Application.Current.TryFindResource("AnalysisFileUsageBookmarks").ToString();
      var result = new StringBuilder();

      result.AppendFormat(text, model.BookmarkCount);

      return result.ToString();
    }

    private string FileSizeLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is FileSizeModel model) )
        return string.Empty;

      var result = new StringBuilder();

      if ( model.IsWindowsEvent )
      {
        result.AppendFormat(Application.Current.TryFindResource("AnalysisFileUsageTotalEvents").ToString(), model.FileSize);
      }
      else
      {
        result.AppendFormat(Application.Current.TryFindResource("AnalysisFileUsageLogCount").ToString(), model.LogCount);
        result.AppendLine();
        result.AppendFormat(Application.Current.TryFindResource("AnalysisFileUsageTotalFileSize").ToString(), model.FileSize / 1024);
      }

      return result.ToString();
    }

    private string FileSessionLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is FileSessionModel model) )
        return string.Empty;

      var text = Application.Current.TryFindResource("AnalysisFileUsageChartYFormat").ToString();
      var str = string.Format(text, model.TimeSpan.Days, model.TimeSpan.Hours, model.TimeSpan.Minutes);
      var result = new StringBuilder();

      result.AppendLine($"Session: {model.SessionCount} - {model.Date.ToString(SettingsHelperController.CurrentSettings.CurrentDateFormat)}");
      result.Append(str);

      if ( model.IsSmartWatch && !model.IsWindowsEvent )
      {
        result.AppendLine();
        result.Append(Application.Current.TryFindResource("AnalysisFileUsageSmartWatchUsed"));
      }

      if ( model.IsWindowsEvent )
      {
        result.AppendLine();
        result.Append(Application.Current.TryFindResource("AnalysisFileUsageWindowsEventLog"));
      }
      return result.ToString();
    }

    private string FileUsageXAxisFormatter(double arg)
    {
      double file = arg + 1;
      return $"{file}";
    }

    private string FileUsageYAxisFormatter(double arg)
    {
      var timeSpan = new TimeSpan((long) (arg * TimeSpan.FromMinutes(15).Ticks));
      var text = Application.Current.TryFindResource("AnalysisFileUsageChartYFormat").ToString();
      var str = string.Format(text, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);

      return str;
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
      if ( !(ItemsControl.ContainerFromElement(ListBox, (DependencyObject) e.OriginalSource) is ListBoxItem item) )
        return;

      foreach ( object lbItem in ListBox.Items )
      {
        if ( !(lbItem is ChartVisibility chart) )
          continue;

        chart.Visibility = Visibility.Collapsed;
      }

      var chartVisibility = (ChartVisibility) item.Content;
      chartVisibility.Visibility = Visibility.Visible;
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
