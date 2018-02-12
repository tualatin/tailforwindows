using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.BaseView
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class Options
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public Options()
    {
      InitializeComponent();

      SourceInitialized += (o, e) =>
      {
        this.HideMinimizeMaximizeButtons();
      };
    }
  }
}
