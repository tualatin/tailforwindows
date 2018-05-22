using System.Windows;


namespace Org.Vs.TailForWin.UI.FloatWindow
{
  /// <summary>
  /// Floating window
  /// </summary>
  public class VsFloatingWindow : Window
  {
    static VsFloatingWindow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VsFloatingWindow), new FrameworkPropertyMetadata(typeof(VsFloatingWindow)));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsFloatingWindow()
    {
      WindowStyle = WindowStyle.None;
      AllowsTransparency = true;
      Style = (Style) Application.Current.TryFindResource("VsFloatingWindowStyle");
    }
  }
}
