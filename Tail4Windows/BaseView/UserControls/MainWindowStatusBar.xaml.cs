using System.Windows;
using System.Windows.Media;


namespace Org.Vs.TailForWin.BaseView.UserControls
{
  /// <summary>
  /// Interaction logic for MainWindowStatusBar.xaml
  /// </summary>
  public partial class MainWindowStatusBar
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public MainWindowStatusBar() => InitializeComponent();

    #region DependencyProperties

    /// <summary>
    /// StatusBar background color property
    /// Default: 104, 33, 122
    /// File loaded: 0, 122, 204
    /// Tailing: 202, 81, 0
    /// take a look at <see cref="Core.Data.Settings.DefaultEnvironmentSettings"/>
    /// </summary>
    public static readonly DependencyProperty StatusBarBackgroundColorProperty = DependencyProperty.Register("StatusBarBackgroundColor", typeof(Brush), typeof(MainWindowStatusBar),
      new PropertyMetadata(new SolidColorBrush(Color.FromRgb(104, 33, 122))));

    /// <summary>
    /// StatusBar background color
    /// </summary>
    public Brush StatusBarBackgroundColor
    {
      get => (Brush) GetValue(StatusBarBackgroundColorProperty);
      set => SetValue(StatusBarBackgroundColorProperty, value);
    }

    #endregion
  }
}
