using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data;
using Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.StatisticModule.UserControls
{
  /// <summary>
  /// Interaction logic for UCMemoryUsageChart.xaml
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public partial class UCMemoryUsageChart : IChartUserControl
  {
    /// <summary>
    /// Memory usage series
    /// </summary>
    public SeriesCollection MemoryUsageSeries
    {
      get;
      set;
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
    /// Analysis collection property <see cref="IStatisticAnalysisCollection"/>
    /// </summary>
    public static readonly DependencyProperty AnalysisCollectionProperty = DependencyProperty.Register(nameof(AnalysisCollection), typeof(IStatisticAnalysisCollection),
      typeof(UCMemoryUsageChart), new PropertyMetadata(null, OnAnalysisCollectionChanged));

    private static void OnAnalysisCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is UCMemoryUsageChart myChart) )
        return;

      myChart.CreateMemoryUsageChart();
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
    /// Standard constructor
    /// </summary>
    public UCMemoryUsageChart()
    {
      InitializeComponent();
      DataContext = this;
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

      var count = 0;

      foreach ( StatisticAnalysisData item in AnalysisCollection )
      {
        memoryUsage.Add(Math.Round((item.SessionEntity.MemoryUsage / 1024d) / 1014, 2));
        upSessionUptime.Add(new DateModel
        {
          TimeSpan = item.SessionEntity.UpTime,
          Value = count
        });
        count++;
      }

      XAxisFormatter = MemoryUsageXAxisFormatter;
      YAxisFormatter = MemoryUsageYAxisFormatter;
      OnPropertyChanged(nameof(MemoryUsageSeries));
    }

    private string MemoryUsageXAxisFormatter(double arg)
    {
      double session = arg + 1;
      //string plural = (int) day == 1 ? Application.Current.TryFindResource("AnalysisMemUsageDay").ToString() : Application.Current.TryFindResource("AnalysisMemUsageDays").ToString();
      return $"{session}.";
    }

    private string MemoryUsageYAxisFormatter(double arg) => $"{arg} MB";

    private string MemoryUsageLabelPoint(ChartPoint arg)
    {
      if ( !(arg.Instance is DateModel model) )
        return string.Empty;

      var result = new StringBuilder();
      result.AppendLine($"{model.TimeSpan.Days:D0}{Application.Current.TryFindResource("AnalysisMemUsageDaysShort")}");
      result.Append($"{model.TimeSpan.Hours:D2}:{model.TimeSpan.Minutes:D2} {Application.Current.TryFindResource("AnalysisMemUsageHoursShort")}");

      return result.ToString();
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
