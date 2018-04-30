using log4net;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for SplitWindowControl.xaml
  /// </summary>
  public partial class SplitWindowControl
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SplitWindowControl));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl() => InitializeComponent();
  }
}
