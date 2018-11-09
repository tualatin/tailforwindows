using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
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
    /// Analysis collection property <see cref="IStatisticAnalysisCollection"/>
    /// </summary>
    public static readonly DependencyProperty AnalysisCollectionProperty = DependencyProperty.Register(nameof(AnalysisCollection), typeof(IStatisticAnalysisCollection),
      typeof(UCFileUsageChart), new PropertyMetadata(null, OnAnalysisCollectionChanged));

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

      OnPropertyChanged(nameof(ChartSeries));
      OnPropertyChanged(nameof(TotalLinesRead));
      OnPropertyChanged(nameof(AverageDailyFileCount));

      if ( _runner == null )
        return;

      _runner.PropertyChanged -= OnRunnerPropertyChanged;
      _runner = null;
    }

    /// <summary>
    /// Analysis collection
    /// </summary>
    public IStatisticAnalysisCollection AnalysisCollection
    {
      get => (IStatisticAnalysisCollection) GetValue(AnalysisCollectionProperty);
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

      await Task.WhenAll(totalLines, averageDailyFileCount).ConfigureAwait(false);
    }

    private async Task CalcTotalLinesReadAsync(IStatisticAnalysisCollection collection) => await Task.Run(() =>
    {
      if ( collection.Count == 0 )
      {
        _totalLinesRead = string.Empty;
        return;
      }

      ulong lines = 0;

      foreach ( StatisticAnalysisData item in collection )
      {
        ulong current = 0;
        current = item.Files.Aggregate(current, (c, i) => c + i.LogCount);
        lines += current;
      }

      _totalLinesRead = $"{lines:N0}";
    }).ConfigureAwait(false);

    private async Task CalcAverageDailyFileCountAsync(IStatisticAnalysisCollection collection) => await Task.Run(() =>
    {
      if ( collection.Count == 0 )
      {
        _averageDailyFileCount = string.Empty;
        return;
      }

      var result = collection.GroupBy(p => p.SessionEntity.Date).Select(p => p.Select(x => x.Files)).ToList();

      if ( result.Count == 0 )
        return;

      var fileCount = 0;

      foreach ( var item in result )
      {
        fileCount += item.Select(p => p.Count).FirstOrDefault();
      }

      decimal average = (decimal) fileCount / result.Count;
      _averageDailyFileCount = $"{average:N1}";
    }).ConfigureAwait(false);

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
  }
}
