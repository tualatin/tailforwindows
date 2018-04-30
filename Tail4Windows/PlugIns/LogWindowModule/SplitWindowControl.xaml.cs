using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using log4net;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for SplitWindowControl.xaml
  /// </summary>
  public partial class SplitWindowControl : INotifyPropertyChanged
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SplitWindowControl));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl() => InitializeComponent();

    /// <summary>
    /// SplitterPosition property
    /// </summary>
    public static readonly DependencyProperty SplitterPrositionProperty = DependencyProperty.Register("SplitterPosition", typeof(double), typeof(SplitWindowControl),
      new PropertyMetadata(OnHeightChanged));

    private static void OnHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is SplitWindowControl control) )
        return;

      if ( e.NewValue == null )
        return;

      control.SplitterPosition = (double) e.NewValue;
      control.OnPropertyChanged(nameof(SplitterPosition));

      LOG.Trace($"{control.SplitterPosition}");
    }

    /// <summary>
    /// Gets/sets SplitterPosition
    /// </summary>
    public double SplitterPosition
    {
      get => (double) GetValue(SplitterPrositionProperty);
      set => SetValue(SplitterPrositionProperty, value);
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
